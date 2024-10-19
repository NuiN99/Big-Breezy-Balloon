using System;
using UnityEngine;

[Serializable]
public class FloatRange
{
    [field: SerializeField] public float Min { get; private set; }
    [field: SerializeField] public float Max { get; private set; }

    public float Lerp(float lerp) => Mathf.Lerp(Min, Max, lerp);

    public FloatRange(float min, float max)
    {
        Min = min;
        Max = max;
    }
}