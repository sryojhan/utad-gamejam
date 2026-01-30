using UnityEngine;
using UnityEditor;
using System.Reflection;

[InitializeOnLoad]
public class VectorHandleTool
{
    static VectorHandleTool()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        GameObject targetObj = Selection.activeGameObject;
        if (targetObj == null) return;

        MonoBehaviour[] scripts = targetObj.GetComponents<MonoBehaviour>();

        foreach (var script in scripts)
        {
            if (script == null) continue;

            FieldInfo[] fields = script.GetType().GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                HandleAttribute attr = field.GetCustomAttribute<HandleAttribute>();

                if (attr != null && (field.FieldType == typeof(Vector3) || field.FieldType == typeof(Vector2)))
                {
                    DrawHandle(script, field, attr);
                }
            }
        }
    }

    private static void DrawHandle(MonoBehaviour script, FieldInfo field, HandleAttribute attr)
    {
        Vector3 currentVal = (Vector3)field.GetValue(script);

        Vector3 worldPos = attr.useLocalSpace ? script.transform.TransformPoint(currentVal) : currentVal;

        EditorGUI.BeginChangeCheck();

        Quaternion handleRotation = attr.useLocalSpace ? script.transform.rotation : Quaternion.identity;
        Vector3 newWorldPos = Handles.PositionHandle(worldPos, handleRotation);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(script, "Move Vector Handle");

            Vector3 newVal = attr.useLocalSpace ? script.transform.InverseTransformPoint(newWorldPos) : newWorldPos;

            field.SetValue(script, newVal);
            EditorUtility.SetDirty(script);
        }

        if (attr.drawConnectionLine)
        {
            Vector3 startLine = script.transform.position;
            Handles.DrawDottedLine(startLine, newWorldPos, 5f);
        }
    }
}