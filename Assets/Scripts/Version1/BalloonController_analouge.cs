using NuiN.NExtensions;
using UnityEngine;

public class BalloonController_analouge : MonoBehaviour
{
    [SerializeField] BalloonMovement_A movement;
    
    PlayerControls _controls;
    float inflateRate = 0.0f;
    float inflateRate_prev = 0.0f;

    void Start()
    {
        PlayerCamera.Instance.AssignTrackingTarget(transform);
    }

    void OnEnable()
    {
        _controls = new PlayerControls();
        _controls.Enable();
    }

    void OnDisable()
    {
        _controls.Disable();
        _controls = null;
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (_controls.Balloon.Move.IsPressed())
        {
            Vector2 input = _controls.Balloon.Move.ReadValue<Vector2>();
            Vector3 moveDirection = ((PlayerCamera.Instance.Forward * input.y) + (PlayerCamera.Instance.Right * input.x)).With(y: 0);

            movement.Move(moveDirection);
        }

        inflateRate_prev = inflateRate;
        inflateRate = _controls.Balloon.Inflate.GetControlMagnitude();
        if (inflateRate > 0.0001f)
        {
            float increase = inflateRate - inflateRate_prev;
            if(increase > 0.0001f) {
                movement.Inflate(increase);
            }
                
        }

        float deflate_rate = _controls.Balloon.Deflate.GetControlMagnitude();
        if(deflate_rate > 0.0001f)
        {
            movement.Deflate(deflate_rate);
        }
    }
}