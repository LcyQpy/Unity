using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class MyEditorWindows : EditorWindow
{    bool showBtn = true;
    [MenuItem("MyWindows/001")]
    public static void ShowMyWindow()
    {
        Debug.Log("只是我的第一个窗口");
        MyEditorWindows window = EditorWindow.GetWindow<MyEditorWindows>("MyEditorWindow");
        window.Show();
        window.minSize = new Vector2(500, 500);
    }

    void OnGUI()
    {
        GUILayout.Label("FindAllMissAssert");
        if(showBtn){
            if(GUILayout.Button("Star")){
                Debug.Log("hhhhhhh");
            }
        }
    }
}