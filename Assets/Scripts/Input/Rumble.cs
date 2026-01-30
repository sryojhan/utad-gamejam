using System;
using UnityEngine;
using UnityEngine.InputSystem;


[Serializable]
public class Rumble
{
    [Serializable]
    public class Settings
    {
        public float low = .1f;
        public float high = .2f;

        public float duration = .2f;
    }

    [SerializeField] private Settings defaultSettings;
    [SerializeField] private Settings linearFallOff;


    [Button]
    public void Begin()
    {
        if (Gamepad.current == null) return;

        Begin(defaultSettings);
    }

    public void Begin(Settings settings)
    {
        Gamepad.current.SetMotorSpeeds(settings.low, settings.high);
        Utils.Delay(settings.duration, Stop);
    }

    public void Stop()
    {
        Gamepad.current.SetMotorSpeeds(0, 0);
    }

}
