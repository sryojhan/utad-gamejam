using System;
using UnityEngine;



public interface IMotionSettings
{
    public MotionSettings Settings { get; }
}

[Serializable]
public class MotionSettings : IMotionSettings
{
    public MotionSettings Settings => this;

    public float duration = 1;
    public float delay = 0;
    public float playFrom = 0;
    public bool useUnscaledTime = false;
    public Interpolation interpolation = new();

    public static MotionSettings Clone(MotionSettings other)
    {
        MotionSettings ms = new()
        {
            duration = other.duration,
            delay = other.delay,
            playFrom = other.playFrom,
            useUnscaledTime = other.useUnscaledTime,
            interpolation = other.interpolation
        };
        return ms;
    }
}
