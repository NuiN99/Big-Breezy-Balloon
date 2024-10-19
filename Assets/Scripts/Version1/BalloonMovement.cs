using System.Collections;
using System.Linq;
using NuiN.NExtensions;
using UnityEngine;

public class BalloonMovement : MonoBehaviour
{
    [Header("Dependencies")] 
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider col;
    
    [Header("Physics")]
    [SerializeField] FloatRange gravityRange;
    [SerializeField] FloatRange dragRange;
    [SerializeField] FloatRange angularDragRange;
    [SerializeField] FloatRange bounceRange;
    [SerializeField] FloatRange frictionRange;
    [SerializeField] float bounceForce;
    
    [Header("Other")]
    [SerializeField] FloatRange speedRange;
    [SerializeField] FloatRange scaleRange;
    [SerializeField] float inflateSpeed;
    [SerializeField] float aimYMult = 4f;
    
    [Header("Deflating")] 
    [SerializeField] float deflateSpeed;
    [SerializeField] FloatRange deflateForceRange;
    
    bool _isDeflating;
    
    float _curSize;
    float _gravity;

    float SizeLerp => _curSize;
    float InverseSizeLerp => 1 - SizeLerp;

    void Start()
    {
        UpdateValues();
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * (_gravity * Time.fixedDeltaTime), ForceMode.Acceleration);
        FaceCameraDirection();
    }
    
    void OnCollisionEnter(Collision other)
    {
        Bounce(other);
    }

    public void Inflate()
    {
        if (_isDeflating) 
            return;
        
        float speed = inflateSpeed * Time.deltaTime;
        _curSize = Mathf.Clamp01(_curSize + speed);
        
        UpdateValues();
    }


    public void Deflate()
    {
        if (!_isDeflating)
        {
            StartCoroutine(DeflateRoutine());
        }
    }

    IEnumerator DeflateRoutine()
    {
        _isDeflating = true;

        while (_curSize > 0)
        {
            _curSize -= deflateSpeed * Time.fixedDeltaTime;
            UpdateValues();

            float force = deflateForceRange.Lerp(SizeLerp) * Time.fixedDeltaTime;
            rb.AddForce(transform.up * force, ForceMode.VelocityChange);
            
            yield return new WaitForFixedUpdate();
        }

        _isDeflating = false;
    }

    void UpdateValues()
    {
        transform.localScale = Vector3.Lerp(Vector3.one * scaleRange.Min, Vector3.one * scaleRange.Max, SizeLerp);
        rb.linearDamping = dragRange.Lerp(SizeLerp);
        rb.angularDamping = angularDragRange.Lerp(SizeLerp);
        col.material.bounciness = bounceRange.Lerp(SizeLerp);
        col.material.dynamicFriction = frictionRange.Lerp(InverseSizeLerp);
        col.material.staticFriction = frictionRange.Lerp(InverseSizeLerp);
        _gravity = gravityRange.Lerp(SizeLerp);
    }

    public void Move(Vector3 direction)
    {
        if (SizeLerp <= 0) return;
        
        float speed = speedRange.Lerp(SizeLerp) * Time.fixedDeltaTime;
        rb.AddForce(direction * speed, ForceMode.Acceleration);
    }

    void Bounce(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        Vector3 reflectDirection = Vector3.Reflect(rb.linearVelocity, contact.normal);
        rb.AddForceAtPosition(contact.point, reflectDirection * SizeLerp * bounceForce);
    }

    void FaceCameraDirection()
    {
        float baseSpringStrength = 10f;
        float damperStrength = 0.75f;

        Vector3 up = transform.up;
        Vector3 fwd = PlayerCamera.Instance.Forward.With(y: PlayerCamera.Instance.Forward.y * aimYMult).normalized;
        
        float angleFromUpright = Vector3.Angle(up, fwd);
    
        float springStrength = baseSpringStrength * (angleFromUpright / 90f);
        springStrength = Mathf.Clamp(springStrength, 0, baseSpringStrength * 2);

        var springTorque = springStrength * Vector3.Cross(up, fwd);
        var dampTorque = damperStrength * -rb.angularVelocity;
        rb.AddTorque(springTorque + dampTorque, ForceMode.Acceleration);
    }
}