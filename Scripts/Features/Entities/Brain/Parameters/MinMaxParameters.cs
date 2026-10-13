using System;
using UnityEngine;

[Serializable]
public class MinMaxParameters
{
    [field: SerializeField]
    [field: Min(0f)]
    public float Min { get; private set; } = 1f;

    [field: SerializeField]
    [field: Min(0f)]
    public float Max { get; private set; } = 10f;
}
