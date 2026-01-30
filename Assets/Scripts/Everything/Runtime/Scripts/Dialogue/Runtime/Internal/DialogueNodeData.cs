using System;
using UnityEngine;

namespace Dialogue.Internal
{
    [Serializable]
    internal class DialogueNodeData : DialogueNode
    {
        [TextArea]
        public string message;
    }
}
