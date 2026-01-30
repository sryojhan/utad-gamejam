using UnityEngine;

using Dialogue.Internal;
using System;

namespace Dialogue
{
    //TODO: refactor this into various classes
    public readonly struct DialogueEntry
    {
        readonly DialogueGraph _asset;

        readonly private bool isValid;

        readonly private bool isChoiceNode;

        readonly private DialogueNode _node;

        readonly private DialogueNodeData _data;
        readonly private DialogueNodeBranch _choice;

        public static DialogueEntry Invalid()
        {
            return new();
        }

        internal DialogueEntry(DialogueNode node, DialogueGraph asset)
        {
            _asset = asset;

            _data = null;
            _choice = null;

            isChoiceNode = false;

            _node = node;
            isValid = false;

            if (!node)
            {
                Debug.LogError("Tried to create a dialogue entry with an invalid dialogue node");
                return;
            }

            if (node is DialogueNodeData data)
            {
                _data = data;
                isValid = true;
            }

            else if (node is DialogueNodeBranch choice)
            {
                _choice = choice;
                isValid = true;
            }
        }

        public DialogueEntry Next()
        {
            DialogueNode node = _asset.GetNextNode(_node);

            if (node == null) return Invalid();

            return new DialogueEntry(node, _asset);
        }

        public bool IsChoice => isChoiceNode;

        public string[] GetChoiceOptions()
        {
            if (!isChoiceNode) return null;

            return null;
        }

        public string GetMessage()
        {
            if (isChoiceNode) return "";

            return _data.message;
        }


        public override bool Equals(object obj)
        {
            return obj is DialogueEntry entry && _node == entry._node;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_data, _choice);
        }

        public static bool operator ==(DialogueEntry entry, DialogueEntry other)
        {
            return entry._node == other._node;
        }

        public static bool operator !=(DialogueEntry entry, DialogueEntry other)
        {
            return !(entry == other);
        }

    }
}
