using NuiN.SpleenTween;
using UnityEngine;

public class BalloonSounds : MonoBehaviour
{
    [SerializeField] BalloonMovement movement;
    
    [SerializeField] AudioSource deflateSource;
    [SerializeField] float deflateVolumeFadeDuration = 0.2f;
    
    [SerializeField] FloatRange maxPitchRange;

    void OnEnable()
    {
        movement.OnStartDeflate += RandomizeDeflateSoundValues;
    }

    void OnDisable()
    {
        movement.OnStartDeflate -= RandomizeDeflateSoundValues;
    }

    void RandomizeDeflateSoundValues()
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
}