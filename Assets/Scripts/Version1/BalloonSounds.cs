using NuiN.ScriptableHarmony.Sound;
using NuiN.SpleenTween;
using UnityEngine;

public class BalloonSounds : MonoBehaviour
{
    [SerializeField] BalloonMovement movement;
    
    [Header("Deflate")]
    [SerializeField] AudioSource deflateSource;
    [SerializeField] FloatRange maxDeflatePitchRange = new(5f, 6f);
    
    [Header("Inflate")]
    [SerializeField] AudioSource inflateSource;
    [SerializeField] float inflateVolume;

    [Header("Impact")]
    [SerializeField] AudioSource impactSource;
    [SerializeField] FloatRange impactSoundPitchRange;
    [SerializeField] float maxImpactForce = 7f;
    [SerializeField] AudioClip[] impactSounds;

    [Header("Wind")] 
    [SerializeField] AudioSource windSource;
    [SerializeField] float maxWindVelocity;
    [SerializeField] FloatRange windVolumeRange;
    [SerializeField] FloatRange windPitchRange;
    
    ITween _inflateVolumeTween;

    void OnEnable()
    {
        movement.OnStartDeflate += PlayDeflateSound;
        movement.OnStartInflate += PlayInflateSound;
        movement.OnStopInflate += StopInflateSound;
        movement.OnColliison += PlayImpactSound;
    }

    void OnDisable()
    {
        movement.OnStartDeflate -= PlayDeflateSound;
        movement.OnStartInflate -= PlayInflateSound;
        movement.OnStopInflate -= StopInflateSound;
        movement.OnColliison -= PlayImpactSound;
    }

    void PlayDeflateSound()
    {
        deflateSource.Stop();

        float remainingDeflateTime = movement.SizeLerp * deflateSource.clip.length;
        if (remainingDeflateTime > 0)
        {
            float pitch = deflateSource.clip.length / remainingDeflateTime;
            if (pitch > maxDeflatePitchRange.Max)
            {
                pitch = maxDeflatePitchRange.Random();
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

    void PlayImpactSound(Collision other)
    {
        float impactForce = other.GetContact(0).impulse.magnitude;
        float volume = impactForce / maxImpactForce;
        impactSource.volume = volume;
        int index =  Mathf.RoundToInt(movement.SizeLerp * (impactSounds.Length - 1));
        impactSource.pitch = impactSoundPitchRange.Random();
        impactSource.PlayOneShot(impactSounds[index]);
    }

    void FixedUpdate()
    {
        // maybe change pitch based on size?
        
        float velLerp = movement.RB.linearVelocity.magnitude / maxWindVelocity;
        windSource.volume = windVolumeRange.Lerp(velLerp);
        windSource.pitch = windPitchRange.Lerp(movement.SizeLerp);
    }
}