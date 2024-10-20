using NuiN.NExtensions;
using NuiN.SpleenTween;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class RigidTitle : MonoBehaviour
{
    [SerializeField] private AudioClip _popSFX;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField, InjectComponent] private Rigidbody RB;
    [SerializeField, InjectComponent] private Collider _collider;
    [SerializeField] private int startDelayMs = 0;
    [SerializeField] private int resetDelaySec = 5;

    private ITween tween;
    private Vector3 startPos;
    private Vector3 respawnPoint;

    private bool returning;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        startPos = transform.position;
        respawnPoint = transform.position;
        respawnPoint.y = -12;
        if(startDelayMs > 0)
            await Task.Delay(startDelayMs);

        SetBounceTween();
    }

    public void OnMouseDown()
    {
        ParticleSystem.EmitParams particleParams = new ParticleSystem.EmitParams()
        {
            position = RB.position,
            applyShapeToPosition = true
        };

        _particleSystem.Emit(particleParams, 20);
        _audioSource.PlayOneShot(_popSFX);
        _collider.enabled = false;
        
        RB.angularVelocity = Vector3.zero;
        RB.linearVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.position = respawnPoint;

        ReturnToStarting();

        //tween = SpleenTween.AddPosAxis(transform, Axis.y, 16.1f, 8f)
        //                    .SetLoop(Loop.None)
        //                    .SetEase(Ease.OutQuad)
        //                    .OnComplete(
        //                        () => {
        //                            _collider.enabled = true;
        //                            SetBounceTween();
        //                    });
    }

    public void OnMouseEnter()
    {
        if (tween != null)
        {
            Debug.Log($"On mouse enter: {name}");
            tween.Stop();
            AddForce();

            ReturnToStarting();
        }
    }

    private void SetBounceTween()
    {
        tween = SpleenTween.AddPosAxis(transform, Axis.y, 0.5f, 1.5f)
                            .SetLoop(Loop.Rewind)
                            .SetEase(Ease.InOutQuad);
    }

    private void AddForce()
    {
        float t = Random.Range(0, Mathf.PI*2);
        Vector3 v = new Vector3(Mathf.Cos(t), Mathf.Sin(t), 0);

        Debug.Log($"Add force: {v} from random t value: {t}");

        RB.AddForce(v * 3, ForceMode.VelocityChange);
    }

    private void ReturnToStarting()
    {
        if (returning)
            return;

        returning = true;

        this.DoAfter(resetDelaySec,
                () =>
                {
                    _collider.enabled = false;
                    RB.angularVelocity = Vector3.zero;
                    RB.linearVelocity = Vector3.zero;

                    tween = SpleenTween.AddPos(transform, startPos - transform.position, 5f)
                                        .SetLoop(Loop.None)
                                        .SetEase(Ease.OutQuad)
                                        .OnComplete(
                                            () => SpleenTween.RotAxis(transform, Axis.z, 0, 1f)
                                                                .OnComplete(
                                                                    () => {
                                                                        returning = false;
                                                                        _collider.enabled = true;
                                                                        SetBounceTween();
                                                                        })
                                        );
                });
    }

    private void OnCollisionEnter(Collision collision)
    {
        ReturnToStarting();
    }
}
