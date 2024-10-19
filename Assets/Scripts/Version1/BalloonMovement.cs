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
    [SerializeField] FloatRange bounceRange;
    [SerializeField] FloatRange frictionRange;
    [SerializeField] float bounceForce;
    
    [Header("Other")]
    [SerializeField] FloatRange speedRange;
    [SerializeField] FloatRange scaleRange;
    [SerializeField] float inflateSpeed;

    [SerializeField] float maxSize;
    
    float _curSize;
    float _gravity;

    float SizeLerp => _curSize / maxSize;
    float InverseSizeLerp => 1 - SizeLerp;

    void Start()
    {
        UpdateValues();
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * (_gravity * Time.fixedDeltaTime), ForceMode.Acceleration);
        
        float baseSpringStrength = 10f; // Base spring strength
        float damperStrength = 0.75f; // Damper strength

        // Calculate the angle from upright
        float angleFromUpright = Vector3.Angle(rb.transform.up, Vector3.up);
    
        // Scale the spring strength based on the angle (you can adjust the scaling factor)
        float springStrength = baseSpringStrength * (angleFromUpright / 90f); // Scale from 0 to base strength
        springStrength = Mathf.Clamp(springStrength, 0, baseSpringStrength * 2); // Cap the maximum strength

        var springTorque = springStrength * Vector3.Cross(rb.transform.up, Vector3.up);
        var dampTorque = damperStrength * -rb.angularVelocity;
        rb.AddTorque(springTorque + dampTorque, ForceMode.Acceleration);
    }
    
    void OnCollisionEnter(Collision other)
    {
        Bounce(other);
    }

    public void Inflate()
    {
        float speed = inflateSpeed * Time.deltaTime;
        _curSize = Mathf.Clamp(_curSize + speed, 0, maxSize);
        
        UpdateValues();
    }

    void UpdateValues()
    {
        transform.localScale = Vector3.Lerp(Vector3.one * scaleRange.Min, Vector3.one * scaleRange.Max, SizeLerp);
        rb.linearDamping = dragRange.Lerp(SizeLerp);
        col.material.bounciness = bounceRange.Lerp(SizeLerp);
        col.material.dynamicFriction = frictionRange.Lerp(InverseSizeLerp);
        col.material.staticFriction = frictionRange.Lerp(InverseSizeLerp);
        _gravity = gravityRange.Lerp(SizeLerp);
    }

    public void Deflate()
    {
        
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
}