using NuiN.SpleenTween;
using UnityEngine;

public class BalloonSounds : MonoBehaviour
{
    [SerializeField] BalloonMovement movement;
    
    [SerializeField] AudioSource deflateSource;
    [SerializeField] AudioSource inflateSource;

    [SerializeField] FloatRange maxPitchRange;

    [SerializeField] float inflateVolume;
    
    ITween _inflateVolumeTween;

    void OnEnable()
    {
        movement.OnStartDeflate += PlayDeflateSound;
        movement.OnStartInflate += PlayInflateSound;
        movement.OnStopInflate += StopInflateSound;
    }

    void OnDisable()
    {
        movement.OnStartDeflate -= PlayDeflateSound;
        movement.OnStartInflate -= PlayInflateSound;
        movement.OnStopInflate -= StopInflateSound;
    }

    void PlayDeflateSound()
    {
        deflateSource.Stop();

        float remainingDeflateTime = movement.SizeLerp * deflateSource.clip.length;
        if (remainingDeflateTime > 0)
        {
            float pitch = deflateSource.clip.length / remainingDeflateTime;
            if (pitch > maxPitchRange.Max)
            {
                pitch = maxPitchRange.Random();
            }

            deflateSource.pitch = pitch;
        }
        else
        {
            deflateSource.pitch = 1f;
        }

        deflateSource.Play();
    }

    void PlayInflateSound()
    {
        if (movement.SizeLerp >= 1f) return;
        
        _inflateVolumeTween?.Stop();
        _inflateVolumeTween = SpleenTween.Vol(inflateSource, 0, inflateVolume, 0.1f);
        
        float totalInflateTime = (1f - movement.SizeLerp) / movement.InflateSpeed;

        if (totalInflateTime > 0)
        {
            float pitch = inflateSource.clip.length / totalInflateTime;
            inflateSource.pitch = pitch;
        }
        else
        {
            inflateSource.pitch = 1f;
        }
        
        inflateSource.Stop();
        inflateSource.Play();
    }

    void StopInflateSound()
    {
        _inflateVolumeTween?.Stop();
        _inflateVolumeTween = SpleenTween.Vol(inflateSource, 0, 0.1f);
    }
}