using MotionModules;
using System;
using System.Collections.Generic;
using UnityEngine;

//TODO: compositor 
public class MotionComposition : MonoBehaviour
{
    public MotionSettings settings;

    public Transform obj;

    [SerializeReference]
    public List<MotionModule> modules = new ();


    [Button]
    public void Play()
    {
        MotionHandler prev = modules[0].Create();

        MotionHandler first = prev;

        for (int i = 1; i < modules.Count; i++)
        {
            int idx = new();
            idx = i;
            MotionHandler next = modules[idx].Create();

            prev.EndCallback(() =>
            {
                next.Play();
            });

            prev = next;
        }

        first.Play();
    }

#if UNITY_EDITOR

    public void PreviewInEditor()
    {
        throw new NotImplementedException("Nor yet");
    }


#endif

    [ContextMenu("Add Module/Transform/Position")]
    public MovementModule AddMovementModule()
    {
        MovementModule mod = new ();
        modules.Add(mod);
        mod.name = "Position module";
        return mod;
    }

    [ContextMenu("Add Module/Transform/Local position")]
    public void AddMovementLocalModule()
    {
        throw new NotImplementedException("Local module");
    }

    [ContextMenu("Add Module/Transform/Scale")]
    public ScaleModule AddScaleModule()
    {
        ScaleModule mod = new();
        modules.Add(mod);
        mod.name = "Scale module";
        return mod;
    }


    [ContextMenu("Add Module/UI/Anchored position")]
    public void AddMovementUIModule()
    {
        throw new NotImplementedException("Anchored position module");
    }

    [ContextMenu("Add Module/UI/Color")]
    public void AddColorModule()
    {
        throw new NotImplementedException("Color module");
    }

}