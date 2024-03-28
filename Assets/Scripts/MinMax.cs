using System;
using UnityEngine;

[Serializable]
public struct MinMax
{
    public float min;
    public float max;

    public float Lerp(float t)
    {
        return Mathf.Lerp(min, max, t);
    }
    public float CorrespondingValue(MinMax other, float otherValue)
    {
        float t = Mathf.InverseLerp(other.min, other.max, otherValue);
        return Lerp(t);
    }
}
