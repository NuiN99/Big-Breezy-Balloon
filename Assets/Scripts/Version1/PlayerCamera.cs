using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }

    public Vector3 Forward => Cam.transform.forward;
    public Vector3 Right => Cam.transform.right;
    
    [field: SerializeField] public CinemachineCamera Cam { get; private set; }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AssignTrackingTarget(Transform target)
    {
        Cam.Target.TrackingTarget = target;
    }
}