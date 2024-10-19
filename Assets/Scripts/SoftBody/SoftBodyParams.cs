using UnityEngine;

[System.Serializable]
public class SoftBodyParams
{
    [field: SerializeField] public float Spring { get; private set; }
    [field: SerializeField] public float Damper { get; private set; }
    [field: SerializeField] public float MinDistance { get; private set; }
    [field: SerializeField] public float MaxDistance { get; private set; }
    [field: SerializeField] public float Tolerance { get; private set; }
    
    public static SoftBodyParams Lerp(SoftBodyParams a, SoftBodyParams b, float t)
    {
        return new SoftBodyParams()
        {
            Spring = Mathf.Lerp(a.Spring, b.Spring, t),
            Damper = Mathf.Lerp(a.Damper, b.Damper, t),
            MinDistance = Mathf.Lerp(a.MinDistance, b.MinDistance, t),
            MaxDistance = Mathf.Lerp(a.MaxDistance, b.MaxDistance, t),
            Tolerance = Mathf.Lerp(a.Tolerance, b.Tolerance, t),
        };
    }
}
