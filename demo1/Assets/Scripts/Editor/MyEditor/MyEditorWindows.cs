using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.Playables;

public class MyEditorWindows : EditorWindow
{    
    // SerializeField 用于确保将视图状态写入窗口
    // 布局文件。这意味着只要窗口未关闭，即使重新启动 Unity，也会保持
    // 状态。如果省略该属性，仍然会序列化/反序列化状态。
    [SerializeField] TreeViewState m_TreeViewState;

    //TreeView 不可序列化，因此应该通过树数据对其进行重建。
    MyAssetTreeView m_MyAssetTreeView;

    private static List<Component> prefabs = new List<Component>(){};

    void OnEnable ()
    {
        //检查是否已存在序列化视图状态（在程序集重新加载后
        // 仍然存在的状态）
        if (m_TreeViewState == null)
            m_TreeViewState = new TreeViewState ();

        m_MyAssetTreeView = new MyAssetTreeView(m_TreeViewState);
    }

    void OnGUI ()
    {
        UpdateAssetsTreeData();
        m_MyAssetTreeView.OnGUI(new Rect(0, 0, position.width, position.height));
    }

    // 将名为 "My Window" 的菜单添加到 Window 菜单
    [MenuItem ("Assets/Find Missing Assest in Prefab", false, 25)]
    static void ShowWindow ()
    {
        // 获取现有打开的窗口；如果没有，则新建一个窗口：
        var window = GetWindow<MyEditorWindows> ();
        window.titleContent = new GUIContent ("Missing Asset in Prefab");
        window.Show ();
    }

    public void UpdateAssetsTreeData(){
        var CurrentPra = AssetDatabase.GetAssetPath(Selection.objects[0]); // 获得点击prefab
        var StrPath = Selection.objects[0];
        m_MyAssetTreeView.Root = new MyAssetTreeViewItem{id = 1, depth = -1, displayName = "root", path = ""}; // root节点

        FindMissAssets(CurrentPra);
        GetMissingList(CurrentPra);
        m_MyAssetTreeView.CollapseAll(); // 折叠所有子列表
        m_MyAssetTreeView.Reload(); // 
    }
    public void FindMissAssets(string CurrentPra){ // 处理子级item列表
        int TempId = 3;
        var firstItem = new MyAssetTreeViewItem{id = 2, depth = 1, displayName = CurrentPra, path = CurrentPra};
        m_MyAssetTreeView.Root.AddChild(firstItem);
        foreach(var i in prefabs){
            var TempItem = new MyAssetTreeViewItem{id = TempId, depth = 2, displayName= i.name, path = AssetDatabase.GetAssetPath(i.gameObject)};
            m_MyAssetTreeView.Root.AddChild(TempItem);
            TempId++;
        }
    }

    private void GetMissingList(string assetPath){
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
                            prefabs.Add(co);
                        }
                    }
                }
            }
        }
    }
}