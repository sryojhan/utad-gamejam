using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Dialogue.Editor
{
    // Esta clase escucha los eventos de arrastrar cables
    public class DialogueNodeConnectorListener : IEdgeConnectorListener
    {
        private DialogueGraphView _graphView;
        private DialogueSearchWindow _searchWindow;

        public DialogueNodeConnectorListener(DialogueGraphView graphView, DialogueSearchWindow searchWindow)
        {
            _graphView = graphView;
            _searchWindow = searchWindow;
        }

        // Se llama si sueltas el cable ENCIMA de otro puerto válido (Conexión normal)
        public void OnDrop(GraphView graphView, Edge edge)
        {
            _graphView.AddElement(edge);
        }

        // Se llama si sueltas el cable en el AIRE (Lo que tú quieres)
        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            _searchWindow.Init(_graphView, _graphView.EditorWindow); // Reiniciamos para asegurar referencias

            // 1. Le pasamos al buscador el puerto desde donde estamos arrastrando
            // edge.output es el puerto de salida del que tiramos
            _searchWindow.PortToConnect = edge.output;

            Vector2 screenMousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            _searchWindow.SpawnPosition = screenMousePosition;
            // 2. Abrimos el menú en la posición del ratón
            SearchWindow.Open(
                new SearchWindowContext(screenMousePosition + new Vector2(130, 0)),
                _searchWindow
            );
        }
    }
}