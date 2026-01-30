using UnityEditor;
using UnityEngine;

namespace Dialogue.Editor
{
    [CustomEditor(typeof(DialogueGraph))]
    public class DialogueCoordinatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 1. Dibuja el inspector por defecto (las listas de nodos y links)
            // Esto es útil para debug, aunque luego podrías ocultarlo si quieres.
            base.OnInspectorGUI();

            GUILayout.Space(20);

            // 2. Botón para abrir el editor
            if (GUILayout.Button("Abrir Editor de Diálogo", GUILayout.Height(40)))
            {
                // 'target' es el objeto que estás inspeccionando actualmente.
                // Lo casteamos a tu clase DialogueCoordinator.
                DialogueGraph selectedAsset = (DialogueGraph)target;

                // Llamamos a la ventana pasándole este asset concreto
                DialogueEditorWindow.Open(selectedAsset);
            }
        }
    }
}