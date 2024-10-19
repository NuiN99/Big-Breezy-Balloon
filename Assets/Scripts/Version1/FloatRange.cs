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

[Serializable]
public class Vector3Range
{
    [field: SerializeField] public Vector3 Min { get; private set; }
    [field: SerializeField] public Vector3 Max { get; private set; }

    public Vector3 Lerp(float lerp) => Vector3.Lerp(Min, Max, lerp);

    public Vector3Range(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }
}