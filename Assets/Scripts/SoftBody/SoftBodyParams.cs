using UnityEngine;

[System.Serializable]
public struct SoftBodyParams
{
    public float spring;
    public float damper;
    public float minDistance;
    public float maxDistance;
    public float tolerance;

    [field: SerializeField] public float Spring { get => spring; private set => spring = value; }
    [field: SerializeField] public float Damper { get => damper; private set => damper = value; }
    [field: SerializeField] public float MinDistance { get => minDistance; private set => minDistance = value; }
    [field: SerializeField] public float MaxDistance { get => maxDistance; private set => maxDistance = value; }
    [field: SerializeField] public float Tolerance { get => tolerance; private set => tolerance = value; }

    public void Lerp(SoftBodyParams a, SoftBodyParams b, float t)
    {
        Spring = Mathf.Lerp(a.Spring, b.Spring, t);
        Damper = Mathf.Lerp(a.Damper, b.Damper, t);
        MinDistance = Mathf.Lerp(a.MinDistance, b.MinDistance, t);
        MaxDistance = Mathf.Lerp(a.MaxDistance, b.MaxDistance, t);
        Tolerance = Mathf.Lerp(a.Tolerance, b.Tolerance, t);
    }

    public SoftBodyParams(float _spring = 100, float _damper = 0.2f, float _minDistance = 0.2f, float _maxDistance = 0.2f, float _tolerance = 0.2f)
    {
        spring = _spring;
        damper = _damper;
        minDistance = _minDistance;
        maxDistance = _maxDistance;
        tolerance = _tolerance;
    }
}
