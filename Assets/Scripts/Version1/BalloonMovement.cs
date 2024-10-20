using System;
using System.Collections;
using NuiN.NExtensions;
using NuiN.SpleenTween;
using UnityEngine;
using Random = UnityEngine.Random;

public class BalloonMovement : MonoBehaviour
{
    public bool IsDeflating { get; private set; }
    public bool IsInflating { get; private set; }
    public float SizeLerp => _curSize;
    public float InverseSizeLerp => 1 - SizeLerp;
    public float InflateSpeed => inflateSpeed;

    public event Action OnStartDeflate = delegate { };
    public event Action OnStartInflate = delegate { };
    public event Action OnStopInflate = delegate { };
    public event Action<Collision> OnColliison = delegate { };
    
    [Header("Dependencies")]
    [SerializeField] SoftBody softBody;
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider col;
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    
    [Header("Physics")]
    [SerializeField] FloatRange gravityRange;
    [SerializeField] FloatRange dragRange;
    [SerializeField] FloatRange angularDragRange;
    [SerializeField] FloatRange bounceRange;
    [SerializeField] FloatRange frictionRange;
    [SerializeField] FloatRange verticalDragRange;
    [SerializeField] FloatRange collisionDampRange;
    [SerializeField] LayerMask groundMask;
    
    [Header("Other")]
    [SerializeField] FloatRange speedRange;
    [SerializeField] Vector3Range scaleRange;
    [SerializeField] FloatRange camZoomRange;
    [SerializeField] float inflateSpeed;
    [SerializeField] float aimYMult = 4f;
    [SerializeField] float minPopSize = 0.2f;

    [Header("Soft Body")] 
    [SerializeField] SoftBodyParams minSoftBodyParams;
    [SerializeField] SoftBodyParams maxSoftBodyParams;
    
    [Header("Deflating")] 
    [SerializeField] float deflateSpeed;
    [SerializeField] FloatRange deflateForceRange;
    [SerializeField] SerializedWaitForSeconds inflateCooldown;
    
    public Rigidbody RB => rb;
    
    public float DistanceFromGround { get; private set; }

    WaitForFixedUpdate _waitForFixedUpdate;
    bool _canInflate = true;
    bool _isAiming;
    float _curSize;
    float _gravity;
    float _verticalDrag;
    SoftBodyParams _softBodyParams;
    
    void Start()
    {
        inflateCooldown.Init();
        _waitForFixedUpdate = new WaitForFixedUpdate();
        UpdateValues();
    }

    void FixedUpdate()
    {
        Rotate(IsDeflating || _isAiming);
        
        softBody.UpdateSprings(_softBodyParams ?? minSoftBodyParams);
        
        softBody.transform.parent.position = rb.position;
        softBody.transform.parent.rotation = rb.rotation;
        softBody.transform.parent.localScale = transform.localScale;
        
        if (IsDeflating) return;
        
        rb.AddForce(Vector3.down * (_gravity * Time.fixedDeltaTime), ForceMode.Acceleration);

        if (Physics.Raycast(transform.position, Vector3.down, 1f, groundMask))
        {
            rb.AddForce(Vector3.up * (_gravity * SizeLerp * Time.fixedDeltaTime * 1.5f), ForceMode.Acceleration);
        }

        const float maxDist = 20f;
        DistanceFromGround = Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, maxDist, groundMask) ? Vector3.Distance(transform.position, groundHit.point) : maxDist;

        if(!IsDeflating)
        {
            rb.linearVelocity = rb.linearVelocity.With(y: rb.linearVelocity.y * _verticalDrag);
        }
    }

    public void Rotate(bool faceCameraFwd)
    {
        float baseSpringStrength = faceCameraFwd ? 17.5f : 15f;
        float damperStrength = faceCameraFwd ? 5f : 1f;

        Vector3 up = transform.up;
        Vector3 fwd = faceCameraFwd 
            ? PlayerCamera.Instance.Forward.With(y: PlayerCamera.Instance.Forward.y * aimYMult).normalized 
            : Vector3.up + (Vector3.ProjectOnPlane(rb.linearVelocity.With(y:0).normalized, Vector3.up) / 5f) + Random.insideUnitSphere / 1.5f;
        
        float angleFromUpright = Vector3.Angle(up, fwd);
    
        float springStrength = baseSpringStrength * (angleFromUpright / 90f);
        springStrength = Mathf.Clamp(springStrength, 0, baseSpringStrength * 2);

        var springTorque = springStrength * Vector3.Cross(up, fwd);
        var dampTorque = damperStrength * -rb.angularVelocity;
        rb.AddTorque(springTorque + dampTorque, ForceMode.Acceleration);
    }
    
    void OnCollisionEnter(Collision other)
    {
        DieOnCollision(other);
        
        float expoLerp = SpleenExt.GetEase(SizeLerp, Ease.OutExpo);
        float collisionDamp = collisionDampRange.Lerp(expoLerp);
        rb.linearVelocity *= collisionDamp;
        
        OnColliison.Invoke(other);
    }

    void OnCollisionStay(Collision other)
    {
        DieOnCollision(other);
    }

    void DieOnCollision(Collision other)
    {
        if (other.gameObject.CompareTag("Death") || (SizeLerp >= minPopSize && other.gameObject.CompareTag("Sharp")))
        {
            DisableVisuals();
            GameEvents.InvokePlayerDied(this);
        }
    }

    public void EnableVisuals()
    {
        meshRenderer.enabled = true;
    }

    void DisableVisuals()
    {
        meshRenderer.enabled = false;
    }

    public void ResetPhysics()
    {
        RB.linearVelocity = Vector3.zero;
        RB.angularVelocity = Vector3.zero;
        _curSize = 0;
        UpdateValues();
    }

    public void Inflate()
    {
        if (!_canInflate) 
            return;

        if (IsInflating == false)
        {
            OnStartInflate.Invoke();
        }
        
        IsInflating = true;
        
        float speed = inflateSpeed * Time.fixedDeltaTime;
        _curSize = Mathf.Clamp01(_curSize + speed);
        
        UpdateValues();
    }

    public void StopInflating()
    {
        OnStopInflate.Invoke();
        IsInflating = false;
    }

    public void Deflate()
    {
        _isAiming = false;

        if (!IsDeflating && SizeLerp > 0)
        {
            OnStartDeflate.Invoke();
            StartCoroutine(DeflateRoutine());
        }
    }

    IEnumerator DeflateRoutine()
    {
        _canInflate = false;
        IsDeflating = true;

        while (_curSize > 0)
        {
            _curSize -= deflateSpeed * Time.fixedDeltaTime;
            UpdateValues();

            float force = deflateForceRange.Lerp(SizeLerp) * Time.fixedDeltaTime;
            rb.AddForce(transform.up * force, ForceMode.VelocityChange);

            yield return _waitForFixedUpdate;
        }
        IsDeflating = false;
        
        yield return inflateCooldown.Wait;

        _canInflate = true;
    }

    void UpdateValues()
    {
        rb.linearDamping = dragRange.Lerp(SizeLerp);
        rb.angularDamping = angularDragRange.Lerp(SizeLerp);
        col.material.bounciness = bounceRange.Lerp(SizeLerp);
        col.material.dynamicFriction = frictionRange.Lerp(InverseSizeLerp);
        col.material.staticFriction = frictionRange.Lerp(InverseSizeLerp);
        _verticalDrag = verticalDragRange.Lerp(SizeLerp);
        _gravity = gravityRange.Lerp(SizeLerp);
        
                
        transform.localScale = scaleRange.Lerp(SizeLerp);
        _softBodyParams = SoftBodyParams.Lerp(minSoftBodyParams, maxSoftBodyParams, SizeLerp);
        
        float lerpBigger = SpleenExt.GetEase(SizeLerp, Ease.InQuint);
        
        PlayerCamera.Instance.SetZoom(camZoomRange.Lerp(lerpBigger));
    }

    public void Move(Vector3 direction)
    {
        if (SizeLerp <= 0) return;
        
        float speed = speedRange.Lerp(SizeLerp) * Time.fixedDeltaTime;
        rb.AddForce(direction * speed, ForceMode.Acceleration);
    }

    public void StartAiming()
    {
        _isAiming = true;
    }
}