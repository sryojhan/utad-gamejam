using System.Collections.Generic;
using UnityEngine;

using Dialogue.Internal;
using System.Collections;

namespace Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/Graph", fileName = "New graph")]
    public class DialogueGraph : ScriptableObject, IEnumerable<DialogueEntry>
    {
        [SerializeReference]
        internal List<DialogueNode> nodes = new();

        [SerializeReference]
        internal List<DialogueLink> links = new();

        public IEnumerator<DialogueEntry> GetEnumerator()
        {
            for (DialogueEntry entry = Begin(); entry != End(); entry = entry.Next())
            {
                yield return entry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public DialogueEntry Begin()
        {
            if (nodes.Count == 0) return DialogueEntry.Invalid();

            return new DialogueEntry(nodes[0], this);
        }

        public DialogueEntry End()
        {
            return DialogueEntry.Invalid();
        }

        private Dictionary<string, DialogueNode> guidTranslateTable;
        private Dictionary<DialogueNode, List<DialogueNode>> connections;

        private void InitialiseConnections()
        {
            connections = new();
            guidTranslateTable = new();

            foreach (DialogueNode node in nodes)
            {
                guidTranslateTable.Add(node.guid, node);
                connections.Add(node, new List<DialogueNode>());
            }

            foreach(DialogueLink link in links)
            {
                connections[guidTranslateTable[link.from]].Add(guidTranslateTable[link.to]);
            }
        }


        internal DialogueNode GetNextNode(DialogueNode node)
        {
            if (connections == null) InitialiseConnections();

            List<DialogueNode> links = connections[node];

            if (links.Count == 0) return null;

            return links[0];
        } 
    }

}
