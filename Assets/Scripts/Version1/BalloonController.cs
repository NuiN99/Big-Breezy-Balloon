using NuiN.NExtensions;
using UnityEngine;

public class BalloonController : MonoBehaviour
{
    [SerializeField] BalloonMovement movement;
    
    PlayerControls _controls;

    void Start()
    {
        PlayerCamera.Instance.AssignTrackingTarget(transform);
    }

    void OnEnable()
    {
        _controls = new PlayerControls();
        _controls.Enable();

        _controls.Balloon.Deflate.performed += _ => movement.Deflate();
    }

    void OnDisable()
    {
        _controls.Disable();
        _controls = null;
    }

    void Update()
    {
        if (_controls.Balloon.Inflate.IsPressed())
        {
            movement.Inflate();
        }
    }

    private void FixedUpdate()
    {
        if (_controls.Balloon.Move.IsPressed())
        {
            Vector2 input = _controls.Balloon.Move.ReadValue<Vector2>();
            Vector3 moveDirection = ((PlayerCamera.Instance.Forward * input.y) + (PlayerCamera.Instance.Right * input.x)).With(y: 0);

            movement.Move(moveDirection);
        }
    }
}