using NuiN.NExtensions;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(SphereCollider))]
public class GoalTrigger : MonoBehaviour
{
    [SerializeField, InjectComponent] Rigidbody RB;
    [SerializeField, InjectComponent] SphereCollider sphereCollider;

#if UNITY_EDITOR
    private void Reset()
    {
        if (RB == null)
            RB = GetComponent<Rigidbody>();

        if(sphereCollider == null)
            sphereCollider = GetComponent<SphereCollider>();

        RB.isKinematic = true;
        sphereCollider.isTrigger = true;
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Timer.OnLevelComplete();
    }
}
