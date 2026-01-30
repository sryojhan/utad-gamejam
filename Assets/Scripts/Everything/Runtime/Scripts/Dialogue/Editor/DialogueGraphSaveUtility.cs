using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

using Dialogue.Internal;

namespace Dialogue.Editor
{
    public class DialogueGraphSaveUtility
    {
        private DialogueGraphView _graphView;
        private DialogueGraph _asset;

        private List<Edge> Edges => _graphView.edges.ToList();
        private List<Node> Nodes => _graphView.nodes.ToList().Cast<Node>().ToList();

        public static DialogueGraphSaveUtility GetInstance(DialogueGraphView graphView, DialogueGraph coordinator)
        {
            return new DialogueGraphSaveUtility { _graphView = graphView, _asset = coordinator };
        }

        public void SaveGraph()
        {
            if (!SaveNodes()) return;
            SaveConnections();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private bool SaveNodes()
        {
            if (_asset == null)
            {
                Debug.LogError("El asset no existe");
                return false;
            }
            _asset.nodes.Clear();
            _asset.links.Clear();
            // Limpiar sub-assets antiguos

            string assetPath = AssetDatabase.GetAssetPath(_asset);
            var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (var asset in assets)
            {
                if (asset is DialogueNode) Object.DestroyImmediate(asset, true);
            }

            foreach (var visualNode in Nodes)
            {
                DialogueNode nodeData;

                if (visualNode.IsBranch)
                {
                    nodeData = ScriptableObject.CreateInstance<DialogueNodeBranch>();
                    nodeData.name = "Branch";
                }
                else
                {
                    var dataNode = ScriptableObject.CreateInstance<DialogueNodeData>();
                    dataNode.message = visualNode.MessageText;
                    nodeData = dataNode;
                    nodeData.name = "Message";
                }

                nodeData.guid = visualNode.GUID;
                nodeData.position = visualNode.GetPosition().position;

                AssetDatabase.AddObjectToAsset(nodeData, _asset);
                _asset.nodes.Add(nodeData);
            }
            return true;
        }

        private void SaveConnections()
        {
            foreach (var edge in Edges)
            {
                var outputNode = edge.output.node as Node;
                var inputNode = edge.input.node as Node;

                var link = new DialogueLink
                {
                    from = outputNode.GUID,
                    description = edge.output.portName,
                    to = inputNode.GUID
                };

                _asset.links.Add(link);
            }
        }

        public void LoadGraph()
        {
            if (_asset == null)
            {
                Debug.LogError("Asset not found!");
                return;
            }

            ClearGraph();
            GenerateNodes();
            ConnectNodes();
        }

        private void ClearGraph()
        {
            foreach (var node in Nodes)
            {
                Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _graphView.RemoveElement(edge));
                _graphView.RemoveElement(node);
            }
        }

        private void GenerateNodes()
        {
            foreach (var nodeData in _asset.nodes)
            {
                NodeType type = nodeData is DialogueNodeBranch ? NodeType.Branch : NodeType.Entry;
                var visualNode = _graphView.CreateNode(type, nodeData.position, shouldFocus: false);
                visualNode.GUID = nodeData.guid;

                if (nodeData is DialogueNodeData d)
                {
                    visualNode.MessageText = d.message;
                    // Actualizar UI del TextField
                    var textFields = visualNode.Query<TextField>().ToList();
                    if (textFields.Count > 0) textFields[0].SetValueWithoutNotify(d.message);
                }

                // Recrear puertos dinámicos para branches
                if (type == NodeType.Branch)
                {
                    var links = _asset.links.Where(x => x.from == nodeData.guid).ToList();
                    foreach (var link in links)
                    {
                        _graphView.AddOutputPort(visualNode, link.description);
                    }
                }
            }
        }

        private void ConnectNodes()
        {
            foreach (var node in _asset.nodes)
            {
                var links = _asset.links.Where(x => x.from == node.guid).ToList();
                foreach (var link in links)
                {
                    var targetNodeGuid = link.to;
                    var baseNodeVisual = Nodes.First(x => x.GUID == node.guid);
                    var targetNodeVisual = Nodes.First(x => x.GUID == targetNodeGuid);

                    var outputPort = (Port)baseNodeVisual.outputContainer.Children().First(x => ((Port)x).portName == link.description);
                    var inputPort = (Port)targetNodeVisual.inputContainer[0];

                    var edge = outputPort.ConnectTo(inputPort);
                    _graphView.AddElement(edge);
                }
            }
        }

    }
}