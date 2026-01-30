using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class EditorUtils
{
    private const int priority = 2;

    [MenuItem("GameObject/UI/Rounded Rectangle/Normal", false, priority)]
    public static void CreateRoundedRectangle() => CreateRoundedRectangleBase("");

    [MenuItem("GameObject/UI/Rounded Rectangle/Small", false, priority)]
    public static void CreateRoundedRectangleSmall() => CreateRoundedRectangleBase("Small");

    [MenuItem("GameObject/UI/Rounded Rectangle/Big", false, priority)]
    public static void CreateRoundedRectangleBig() => CreateRoundedRectangleBase("Big");

    [MenuItem("GameObject/UI/Rounded Rectangle/Left", false, priority + 1)]
    public static void CreateRoundedRectangleLeft() => CreateRoundedRectangleBase("Left Small");



    private static void CreateRoundedRectangleBase(string variant = "")
    {
        if (!string.IsNullOrWhiteSpace(variant)) variant = " " + variant;

        EditorApplication.ExecuteMenuItem("GameObject/UI/Image");

        GameObject go = Selection.activeGameObject;

        if (go == null) return;

        go.name = "Rounded Rectangle"; 

        if (go.TryGetComponent(out Image image))
        {
            Sprite foundSprite = FindExactAsset<Sprite>("Rounded Rect" + variant);
            if (foundSprite != null)
            {
                image.sprite = foundSprite;
                image.type = Image.Type.Sliced;
            }
            else
            {
                Debug.LogWarning("'RoundedRect' sprite was not found.");
            }
        }
    }

    public static string[] NameToGUIDs<T>(string assetName)
    {
        return AssetDatabase.FindAssets(assetName + $" t:{typeof(T).Name}");
    }

    public static T FindAssetByName<T>(string assetName) where T : Object
    {
        string[] guids = NameToGUIDs<T>(assetName);
        if (guids.Length == 0) return null;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }

    public static T FindExactAsset<T>(string exactName) where T : Object
    {
        string[] guids = NameToGUIDs<T>(exactName);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string filename = Path.GetFileNameWithoutExtension(path);

            if (filename.Equals(exactName, System.StringComparison.OrdinalIgnoreCase))
            {
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }
        }

        return null;
    }

}
