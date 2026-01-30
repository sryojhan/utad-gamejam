using System.Collections.Generic;
using UnityEditor; 
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

using Dialogue.Internal;

namespace Dialogue.Editor
{
    public class DialogueSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueGraphView _graphView;
        private EditorWindow _window; // <--- Añadimos referencia a la ventana
        private Texture2D _indentIcon;

        public Port PortToConnect;
        public Vector2 SpawnPosition;

        // Modificamos Init para pedir la ventana
        public void Init(DialogueGraphView graphView, UnityEditor.EditorWindow window)
        {
            _graphView = graphView;
            _window = window;
            _indentIcon = new Texture2D(1, 1);
            _indentIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            // (Este método se queda igual que antes)
            return new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create dialogue node"), 0),

                new SearchTreeEntry(new GUIContent("Message", _indentIcon))
                {
                    userData = new DialogueNodeData(), level = 1
                },
                new SearchTreeEntry(new GUIContent("Branch", _indentIcon))
                {
                    userData = new DialogueNodeBranch(), level = 1
                }
            };
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var windowMousePosition = SpawnPosition - _window.position.position;
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(windowMousePosition);

            // Variable para guardar el nodo recién creado
            Node createdNode = null;

            switch (SearchTreeEntry.userData)
            {
                case DialogueNodeData _:
                    createdNode = _graphView.CreateNode(NodeType.Entry, graphMousePosition);
                    break;
                case DialogueNodeBranch _:
                    createdNode = _graphView.CreateNode(NodeType.Branch, graphMousePosition);
                    break;
            }

            // --- NUEVO: Lógica de Auto-Conexión ---
            if (createdNode != null && PortToConnect != null)
            {
                // 1. Buscamos el puerto de entrada del nuevo nodo (Input)
                var inputPort = (Port)createdNode.inputContainer[0];

                // 2. Creamos el cable
                var edge = PortToConnect.ConnectTo(inputPort);
                _graphView.AddElement(edge);

                // 3. Limpiamos la referencia para la próxima vez
                PortToConnect = null;
            }

            return true;
        }
    }
}