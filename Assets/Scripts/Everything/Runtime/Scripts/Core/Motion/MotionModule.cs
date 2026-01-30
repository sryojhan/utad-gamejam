using System;
using UnityEngine;

namespace MotionModules
{
    [Serializable]
    public abstract class MotionModule
    {
        public string name;

        public MotionSettings settings;
        public abstract MotionHandler Create();
    }


    [Serializable]
    public class MovementModule : MotionModule
    {
        public Transform target;
        public Vector3 position;

        public override MotionHandler Create()
        {
            return Motion.Move(target, position, settings);
        }
    }

    [Serializable]
    public class ScaleModule : MotionModule
    {
        public Transform target;
        public Vector3 scale;

        public override MotionHandler Create()
        {
            return Motion.Scale(target, scale, settings);
        }
    }

}