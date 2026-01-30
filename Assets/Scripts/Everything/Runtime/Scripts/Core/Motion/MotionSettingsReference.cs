using UnityEngine;

[CreateAssetMenu(menuName = "Custom Motion settings")]
public class MotionSettingsReference : ScriptableObject, IMotionSettings
{
    public MotionSettings settings;
    public MotionSettings Settings => settings;
}

