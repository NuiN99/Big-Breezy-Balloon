using System.Collections.Generic;
using NuiN.NExtensions;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    [SerializeField] float force;
    BalloonMovement _balloon;

    void Awake()
    {
        _balloon = FindFirstObjectByType<BalloonMovement>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("BalloonCollider"))
        {
            _balloon.RB.AddForce(transform.up * (force * _balloon.SizeLerp * Time.fixedDeltaTime));
        }
    }
}