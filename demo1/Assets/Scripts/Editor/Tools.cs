using System;
using UnityEngine;
using UnityEditor;

public class ResTool : Editor
{
    [MenuItem("Tools/清理prefab中所有Missing的脚本")]
    public static void ClearAllPrefabMissingComponents()
    {
        EditorUtility.DisplayProgressBar("Modify Prefab", "Please wait...", 0);
        string[] assetGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Resources/Prefab" });
        Debug.Log(assetGUIDs);
        try
        {
            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
                // Debug.Log($"path:{path}");
                GameObject pre = PrefabUtility.LoadPrefabContents(path);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{e}");
        }

        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
}