using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[Serializable]
public class SceneObject
{
    [SerializeField]
    private string m_guid;

    [SerializeField]
    private string m_lastStored_name;

    public string GetGUID => m_guid;


    public static implicit operator string(SceneObject sceneObject)
    {
        return sceneObject.m_lastStored_name;
    }

}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneObject))]
public class SceneObjectEditor : PropertyDrawer
{
    protected SceneAsset GetSceneAsset(string guid)
    {
        if (string.IsNullOrEmpty(guid))
            return null;

        string assetPath = AssetDatabase.GUIDToAssetPath(guid);

        if (!string.IsNullOrEmpty(assetPath))
        {
            return AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
        }

        return null;
    }

    protected bool IsDefinedInBuildSettings(string guid)
    {
        if (string.IsNullOrEmpty(guid))
            return false;

        foreach (var sceneInBuild in EditorBuildSettings.scenes)
        {
            if (sceneInBuild.guid.ToString().Equals(guid, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty guidProperty = property.FindPropertyRelative("m_guid");
        SerializedProperty nameProperty = property.FindPropertyRelative("m_lastStored_name");

        SceneAsset currentScene = GetSceneAsset(guidProperty.stringValue);

        EditorGUI.BeginChangeCheck();

        SceneAsset newScene = EditorGUI.ObjectField(position, label, currentScene, typeof(SceneAsset), false) as SceneAsset;

        if (EditorGUI.EndChangeCheck())
        {
            if (newScene == null)
            {
                guidProperty.stringValue = "";
                nameProperty.stringValue = "";
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(newScene);
                string newGuid = AssetDatabase.AssetPathToGUID(path);

                if (newGuid != guidProperty.stringValue)
                {
                    if (!IsDefinedInBuildSettings(newGuid))
                    {
                        Debug.LogWarning("The scene " + newScene.name + " cannot be used. To use this scene add it to the build settings for the project.");
                    }
                    else
                    {
                        guidProperty.stringValue = newGuid;
                        nameProperty.stringValue = newScene.name;
                    }
                }
            }
        }

        if (newScene.name != nameProperty.stringValue)
        {
            Debug.Log("La escena ha cambiado de nombre");
            nameProperty.stringValue = newScene.name;
        }
    }
}
#endif

