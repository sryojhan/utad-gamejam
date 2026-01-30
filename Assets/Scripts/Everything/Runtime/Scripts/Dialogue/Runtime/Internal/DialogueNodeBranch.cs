using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    internal class DialogueNodeBranch : DialogueNode
    {
        [HideInInspector] public List<string> choices = new List<string>();
    }
}
