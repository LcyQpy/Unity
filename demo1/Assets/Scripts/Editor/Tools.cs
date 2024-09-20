using System;
using UnityEngine;
using UnityEditor;

public class ResTool : Editor
{
    [MenuItem("Tools/tset")]
    public static void SearchMissResources(){
        // 查找Assets/Resources/Prefab 所有为Prefab类型的资源的GUID
        string[] assetGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Resources/Prefab" });
        if(assetGUIDs.Length == 0){
            Debug.LogError("No Assets, Please check it!");
        }
    }

    [MenuItem("Tools/清理prefab中所有Missing的脚本")]
    public static void ClearAllPrefabMissingComponents()
    {
        EditorUtility.DisplayProgressBar("Modify Prefab", "Please wait...", 0);
        string[] assetGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Res/Prefab" });
        try
        {
            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
                // Debug.Log($"path:{path}");
                GameObject pre = PrefabUtility.LoadPrefabContents(path);

                DeleteRecursive(pre, (go) =>
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                });
                PrefabUtility.SaveAsPrefabAssetAndConnect(pre, path, InteractionMode.AutomatedAction);

                EditorUtility.DisplayProgressBar($"清理Prefab丢失组件", $"当前{pre}, {i}/{assetGUIDs.Length}", i / (float)assetGUIDs.Length);
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

    /// <summary>
    /// 遍历所有子节点
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="action"></param>
    static void DeleteRecursive(GameObject obj, Action<GameObject> action)
    {
        action(obj);

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            DeleteRecursive(obj.transform.GetChild(i).gameObject, action);
        }
    }    
}