using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;



public class MyEditorWindows : EditorWindow
{    bool showBtn = true;
    [MenuItem("MyWindows/001")]
    public static void ShowMyWindow()
    {
        MyEditorWindows window = EditorWindow.GetWindow<MyEditorWindows>("MyEditorWindow");
        window.Show();
        window.minSize = new Vector2(500, 500);
    }

    void OnGUI()
    {
        GUILayout.Label("FindAllMissAssert");
        GUILayout.Box("丢失物品列表");
        if(showBtn){
            if(GUILayout.Button("Star")){
                test();
            }
        }
    }

    private void test(){
        string[] paths = Directory.GetFiles("Assets","*.prefab",SearchOption.AllDirectories);
        if(paths.Length > 0){
            foreach(var _path in paths){
                GameObject tempObj = AssetDatabase.LoadAssetAtPath<GameObject>(_path);
                Component[] components = tempObj.GetComponentsInChildren<Component>();
                if(components.Length > 0){
                    foreach(var co in components){
                        SerializedObject so = new SerializedObject(co);
                        var iter = so.GetIterator();//拿到迭代器
                        while(iter.NextVisible(true)){
                            if(iter.propertyType == SerializedPropertyType.ObjectReference){
                                if(iter.objectReferenceValue == null && iter.objectReferenceInstanceIDValue != 0){
                                    Debug.Log(iter.name);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}