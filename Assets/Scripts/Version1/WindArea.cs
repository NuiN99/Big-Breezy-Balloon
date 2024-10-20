using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    [SerializeField] float force;
    Rigidbody _balloonRB;

    void Awake()
    {
        _balloonRB = FindFirstObjectByType<BalloonController>().GetComponent<Rigidbody>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("BalloonCollider"))
        {
            _balloonRB.AddForce(transform.up * force, ForceMode.Acceleration);
        }
    }
}