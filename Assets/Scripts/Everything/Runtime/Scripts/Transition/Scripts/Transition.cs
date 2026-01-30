using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SceneTransition
{
    [CreateAssetMenu (fileName = "new Scene Transition", menuName = "Scene Transition")]
    public class Transition : ScriptableObject
    {
        [Serializable]
        public class TransitionInfo
        {
            public bool enabled = true;
            public Texture2D gradientMask;
            public float duration = 1;
            public bool invert = false;
            public Interpolation interpolation;
        }

        [Header("In transition")]
        public TransitionInfo In;

        [Header("Middle screen")]
        public         Color backgroundColor = Color.white;
        public     Texture2D background;
        public         float middleScreenDuration = 1;

        [Header("Out transition")]
        public TransitionInfo Out;
    }

}