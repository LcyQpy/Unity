using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;


public class MyEditorWindows : EditorWindow
{    
    // SerializeField 用于确保将视图状态写入窗口
    // 布局文件。这意味着只要窗口未关闭，即使重新启动 Unity，也会保持
    // 状态。如果省略该属性，仍然会序列化/反序列化状态。
    [SerializeField] TreeViewState m_TreeViewState;

    //TreeView 不可序列化，因此应该通过树数据对其进行重建。
    MyAssetTreeView m_MyAssetTreeView;

    public Vector2 win;
    private static List<Transform> prefabs = new List<Transform>(){}; // 丢失组件所在节点

    public static UnityEngine.Object hasTar; // 选中预制体
    public static Transform[] transforms; // 所有节点
    public int currId; // 当前最大ID


    void OnEnable ()
    {
        //检查是否已存在序列化视图状态（在程序集重新加载后
        // 仍然存在的状态）
        if (m_TreeViewState == null)
            m_TreeViewState = new TreeViewState ();
        m_MyAssetTreeView = new MyAssetTreeView(m_TreeViewState);
    }
    // 将名为 "My Window" 的菜单添加到 Window 菜单
    [MenuItem ("Assets/Find Missing Assest in Prefab", false, 25)]
    public static void ShowWindow () 
    {
        // 全局只获取一次预制体
        hasTar = Selection.objects[0];
        transforms = hasTar.GetComponentsInChildren<Transform>();
        // 获取现有打开的窗口；如果没有，则新建一个窗口：
        var window = GetWindow<MyEditorWindows> ();
        window.titleContent = new GUIContent ("Missing Asset in Prefab");
        window.Show();
    }
    void OnGUI ()
    {
        UpdateAssetsTreeData();
        m_MyAssetTreeView.OnGUI(new Rect(win.x, win.y, position.width, position.height));
    }

    public void UpdateAssetsTreeData(){
        var CurrentPra = AssetDatabase.GetAssetPath(hasTar);
        m_MyAssetTreeView.Root = new MyAssetTreeViewItem{id = 0, depth = -1, displayName = "root", path = ""}; // root节点
        DrawNodeTree(transforms); // 获取所有节点并组装成树
        //FindMissAssets(CurrentPra);
        GetMissingList(CurrentPra); // 获取丢失组件的节点
        m_MyAssetTreeView.Reload(); 
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
                            prefabs.Add(co.GetComponent<Transform>());
                            var trans = co.GetComponent<Transform>();
                            var pid = Array.IndexOf(transforms, trans);
                            var parentItem = m_MyAssetTreeView._FindItem(pid+1, m_MyAssetTreeView.Root);
                            var misItem = new MyAssetTreeViewItem{id = ++currId, depth = parentItem.depth + 1, displayName = iter.name, hasMissing = true};
                            parentItem.AddChild(misItem);
                        }
                    }
                }
            }
        }
    }

    private void DrawNodeTree(Transform[] trans){
        // GameObject tempObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        // transforms = tempObj.GetComponentsInChildren<Transform>(); // 所有节点列表
        for(int i = 0; i < trans.Length; i++){
            var item = new MyAssetTreeViewItem{id = i + 1, depth = GetDepth(trans[i], 1), displayName = trans[i].name, path = AssetDatabase.GetAssetPath(trans[i].gameObject), hasMissing = false};
            // 根据id获得父节点
            if(i == 0){
                item.parent = m_MyAssetTreeView.Root;
            }else{
                var _pid = Array.IndexOf(trans, trans[i].parent);
                item.parent = m_MyAssetTreeView._FindItem(_pid + 1, m_MyAssetTreeView.Root);
            }
            item.parent.AddChild(item);
            currId = i + 1;
        }
    }

    private int GetDepth(Transform transform, int depth){
        if(transform.parent){
            depth = depth + 1;
            return GetDepth(transform.parent, depth);
        }else{
            return depth;
        }
    }
}
