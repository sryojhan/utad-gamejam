using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue.Editor
{
    public enum NodeType { Entry, Branch }

    public class DialogueGraphView : GraphView
    {
        DialogueSearchWindow _searchWindow;
        public UnityEditor.EditorWindow EditorWindow;
        public DialogueGraphView(UnityEditor.EditorWindow window)
        {
            EditorWindow = window;

            SetupZoom(0.1f, 8.0f);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();


            AddSearchWindow();

            graphViewChanged = OnGraphViewChanged;
        }

        private void AddSearchWindow()
        {
            _searchWindow = ScriptableObject.CreateInstance<DialogueSearchWindow>();
            _searchWindow.Init(this, EditorWindow);

            nodeCreationRequest = ctx =>
            {
                _searchWindow.SpawnPosition = ctx.screenMousePosition;
                SearchWindow.Open(new SearchWindowContext(ctx.screenMousePosition), _searchWindow);
            };
        }

        // Método principal de fábrica de nodos visuales
        public Node CreateNode(NodeType type, Vector2 position, bool shouldFocus = true)
        {
            var node = new Node
            {
                GUID = Guid.NewGuid().ToString(),
                style = { left = position.x, top = position.y }
            };

            // Puerto de Entrada (Común)
            var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Previous";
            node.inputContainer.Add(inputPort);

            if (type == NodeType.Entry)
            {
                node.title = "Message";
                node.IsBranch = false;

                // TextField para el mensaje
                var textField = new TextField("");

                textField.SetValueWithoutNotify(node.MessageText);
                textField.multiline = true;

                textField.style.maxWidth = 240;
                textField.style.minWidth = 240;

                textField.style.whiteSpace = WhiteSpace.Normal;

                textField.AddToClassList("unity-base-text-field__input");

                textField.RegisterValueChangedCallback(evt => { node.MessageText = evt.newValue; });
                node.extensionContainer.Add(textField);

                //auto-focus next frame

                if (shouldFocus)
                {
                    node.schedule.Execute(() =>
                    {
                        textField.Q("unity-text-input").Focus();
                    });
                }

                // Salida fija
                AddOutputPort(node, "Next");
            }
            else if (type == NodeType.Branch)
            {
                node.title = "Branch";
                node.IsBranch = true;

                // Botón para añadir salidas dinámicas
                var button = new Button(() => { AddOutputPort(node, "Option"); })
                {
                    text = "Add option"
                };
                node.titleContainer.Add(button);
            }

            node.RefreshExpandedState();
            node.RefreshPorts();

            AddElement(node);
            return node;
        }

        public void AddOutputPort(Node node, string portNameOverride)
        {
            var port = GeneratePort(node, Direction.Output);

            // Si es branch, permitimos borrar el puerto
            if (node.IsBranch)
            {
                var deleteButton = new Button(() => RemovePort(node, port)) { text = "X" };
                port.contentContainer.Add(deleteButton);
            }

            port.portName = portNameOverride;
            node.outputContainer.Add(port);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private void RemovePort(Node node, Port port)
        {
            var targetEdge = port.connections;
            if (targetEdge != null)
            {
                foreach (var edge in targetEdge)
                {
                    edge.input.Disconnect(edge);
                    RemoveElement(edge);
                }
            }
            node.outputContainer.Remove(port);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private Port GeneratePort(Node node, Direction dir, Port.Capacity cap = Port.Capacity.Single)
        {
            //return node.InstantiatePort(Orientation.Horizontal, dir, cap, typeof(float));
            var port = node.InstantiatePort(Orientation.Horizontal, dir, cap, typeof(float));

            // 2. Definimos que este puerto escuchará eventos de "Soltar cable"
            // Solo necesitamos esto para puertos de SALIDA (desde donde arrastras hacia el vacío)
            if (dir == Direction.Output)
            {
                port.AddManipulator(new EdgeConnector<Edge>(new DialogueNodeConnectorListener(this, _searchWindow)));
            }

            return port;

        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node)
                    compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }


        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // Verificamos si hay elementos para borrar
            if (graphViewChange.elementsToRemove != null)
            {
                // Iteramos sobre lo que se va a borrar
                foreach (var element in graphViewChange.elementsToRemove)
                {
                    // Si lo que estamos borrando es un Nodo...
                    if (element is Node node)
                    {
                        var edgesToRemove = new List<Edge>();

                        // 1. Buscamos conexiones en los puertos de ENTRADA
                        if (node.inputContainer != null)
                        {
                            foreach (Port port in node.inputContainer.Children().Cast<Port>())
                            {
                                if (port.connections != null)
                                    edgesToRemove.AddRange(port.connections);
                            }
                        }

                        // 2. Buscamos conexiones en los puertos de SALIDA
                        if (node.outputContainer != null)
                        {
                            foreach (Port port in node.outputContainer.Children().Cast<Port>())
                            {
                                if (port.connections != null)
                                    edgesToRemove.AddRange(port.connections);
                            }
                        }

                        // 3. Añadimos esos cables a la lista de borrado oficial
                        foreach (var edge in edgesToRemove)
                        {
                            // Comprobamos que no esté ya en la lista para no duplicar
                            if (!graphViewChange.elementsToRemove.Contains(edge))
                            {
                                graphViewChange.elementsToRemove.Add(edge);
                            }
                        }
                    }

                    // Opcional: Si borras un cable, aquí podrías limpiar datos extra si quisieras
                }
            }

            return graphViewChange;
        }
    }
}