using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue.Editor
{
    public class DialogueEditorWindow : EditorWindow
    {
        private DialogueGraphView _graphView;
        private DialogueGraph _asset;

        [MenuItem("Dialogue/Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<DialogueEditorWindow>();
            window.titleContent = new GUIContent("Dialogue Editor");
        }
        public static void Open(DialogueGraph dialogueCoordinator)
        {
            var window = GetWindow<DialogueEditorWindow>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window._asset = dialogueCoordinator;

            window.UpdateEditorView();
        }

        public void LoadAsset()
        {
            var saveUtility = DialogueGraphSaveUtility.GetInstance(_graphView, _asset);
            saveUtility.LoadGraph(); 
        }

        private void UpdateEditorView()
        {
            rootVisualElement.Clear();

            if (_asset == null)
            {
                DrawEmptyState();
            }
            else
            {
                DrawGraphState();
            }
        }

        private void DrawEmptyState()
        {
            // Creamos un contenedor que ocupe todo
            var container = new VisualElement();
            container.StretchToParentSize();

            // Usamos Flexbox para centrar contenido vertical y horizontalmente
            container.style.alignItems = Align.Center;      // Centro horizontal
            container.style.justifyContent = Justify.Center; // Centro vertical
            container.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f); // Fondo gris oscuro profesional

            // El Texto de aviso
            var label = new Label("Invalid Dialogue Coordinator");
            label.style.fontSize = 20;
            label.style.color = new Color(0.7f, 0.2f, 0.2f); // Rojo suave
            label.style.unityFontStyleAndWeight = FontStyle.Bold;

            // Un texto pequeño de ayuda debajo
            var subLabel = new Label("Select a DialogueCoordinator asset and click 'Open Editor'");
            subLabel.style.fontSize = 12;
            subLabel.style.color = Color.gray;
            subLabel.style.marginTop = 10;

            container.Add(label);
            container.Add(subLabel);
            rootVisualElement.Add(container);
        }

        // Caso B: Todo correcto, pintamos el editor
        private void DrawGraphState()
        {
            ConstructGraphView();
            GenerateToolbar();

            LoadAsset();
        }


        private void OnEnable()
        {
            UpdateEditorView();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }

        private void ConstructGraphView()
        {
            _graphView = new DialogueGraphView(this)
            {
                name = "Dialogue Graph"
            };
            _graphView.style.flexGrow = 1;
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new UnityEditor.UIElements.Toolbar();

            toolbar.Add(new UnityEditor.UIElements.ToolbarButton(() =>
            {
                var saveUtility = DialogueGraphSaveUtility.GetInstance(_graphView, _asset);
                saveUtility.SaveGraph();
            })
            { text = "Save" });


            rootVisualElement.Add(toolbar);
        }
    }
}