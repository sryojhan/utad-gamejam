using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEffectController : Singleton<ScreenEffectController>
{
    [Serializable]
    public struct ScreenEffect
    {
        public string name;
        public ShaderPropertyAnimation controller;
    }

    [SerializeField]
    ScreenEffect[] screenEffects;

    private readonly Map<ScreenEffect> orderedEffects = new();

    private void Awake()
    {
        foreach (ScreenEffect effect in screenEffects)
            orderedEffects.Add(effect.name, effect);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();

        ResetEffects();
    }

    public void Apply(string name)
    {
        if (orderedEffects.TryGetValue(name, out ScreenEffect effect))
        {
            effect.controller.TurnOn(this);
        }

        else
        {
            Debug.LogError($"ScreenEffectController::Apply: Could not locate effect {name}");
        }
    }

    public void Remove(string name)
    {
        if (orderedEffects.TryGetValue(name, out ScreenEffect effect))
        {
            effect.controller.TurnOff(this);
        }

        else
        {
            Debug.LogError($"ScreenEffectController::Remove: Could not locate effect {name}");
        }
    }

    [Button]
    public void ResetEffects()
    {
        foreach (ScreenEffect effect in screenEffects)
            effect.controller.Reset();
    }

}
