using System;
using UnityEngine;

namespace Dialogue.Internal
{
    [Serializable]
    internal class DialogueLink
    {
        public string description;
        [HideInInspector]
        public string from;
        [HideInInspector]
        public string to;
    }
}
