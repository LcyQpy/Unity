using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Overlays;



public class MyEditorWindows : EditorWindow
{    
    bool showBtn = true;
    private string assetPath;
    private Vector3 scroll = Vector3.zero;
    private static Dictionary<UnityEngine.Object, List<UnityEngine.Object>> prefabs = new Dictionary<UnityEngine.Object, List<UnityEngine.Object>>();
    [MenuItem("Assets/FindMissingAssets", false, 25)]
    public static void ShowMyWindow()
    {
        
        MyEditorWindows window = EditorWindow.GetWindow<MyEditorWindows>("MyEditorWindow");
        window.Show();
        window.minSize = new Vector2(200, 20);
    }

    void OnGUI()
    {
        scroll =  EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("GameObject");
        GUILayout.Label("MissingAsset");
        EditorGUILayout.EndHorizontal();
        foreach(var cps in prefabs){
            EditorGUILayout.BeginVertical();
            foreach(var cp in cps.Value){
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(cps.Key, typeof(GameObject), true);
                if(cp){
                    EditorGUILayout.ObjectField(cp, cp.GetType(), true);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        if(showBtn){
            if(GUILayout.Button("find missing asset")){
                assetPath = "";
                UpdatePathInfo();
                GetMissingList();
            }
        }
    }

    private void GetMissingList(){
        prefabs.Clear();
        GameObject tempObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        Component[] components = tempObj.GetComponentsInChildren<Component>();
        if(components.Length > 0){
            foreach(var co in components){
                SerializedObject so = new SerializedObject(co);
                var iter = so.GetIterator();
                while(iter.NextVisible(true)){
                    if(iter.propertyType == SerializedPropertyType.ObjectReference){
                        if(iter.objectReferenceValue == null && iter.objectReferenceInstanceIDValue != 0){
                            if(prefabs.ContainsKey(tempObj)){
                                prefabs[tempObj].Add(co);
                            }else{
                                prefabs.Add(tempObj, new List<Object>(){ co });
                            }
                        }
                    }
                }
            }
        }
    }

    private void UpdatePathInfo(){
        if(Selection.objects.Length == 1){
            assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
        }else{
            EditorUtility.DisplayDialog("Tips", "please, Select a Prefab!", "exit");
        }
    }
}