using System;
using UnityEngine;
using UnityEditor;

public class ResTool : Editor
{
    [MenuItem("Tools/FindMissing")]
    public static void ClearAllPrefabMissingComponents()
    {
        string[] assetGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Resources/Prefab" });
        try
        {
            Debug.Log("文件夹预制体：" + assetGUIDs.Length);
            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
                Debug.Log(path + " Asset GUID:" + assetGUIDs[i]);
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