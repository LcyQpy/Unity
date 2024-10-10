using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using NUnit.Framework;


public class MyEditorWindows : EditorWindow
{    
    // SerializeField 用于确保将视图状态写入窗口
    // 布局文件。这意味着只要窗口未关闭，即使重新启动 Unity，也会保持
    // 状态。如果省略该属性，仍然会序列化/反序列化状态。
    [SerializeField] TreeViewState m_TreeViewState;

    //TreeView 不可序列化，因此应该通过树数据对其进行重建。
    MyAssetTreeView m_MyAssetTreeView;

    private Vector2 win;
    private static UnityEngine.Object hasTar; // 选中预制体
    private static Transform[] transforms; // 所有节点
    private int currId; // 当前最大ID


    void OnEnable ()
    {
        if (m_TreeViewState == null)
            m_TreeViewState = new TreeViewState ();
        m_MyAssetTreeView = new MyAssetTreeView(m_TreeViewState);
    }
    
    [MenuItem ("Assets/Find Missing Assest in Prefab", false, 25)]
    private static void ShowWindow () 
    { 
        // 获取现有打开的窗口；如果没有，则新建一个窗口：
        var window = GetWindow<MyEditorWindows> ();
        window.titleContent = new GUIContent ("Missing Asset in Prefab");
        window.Show();
        // 全局只获取一次预制体
        hasTar = Selection.objects[0];
        transforms = hasTar.GetComponentsInChildren<Transform>();
    }
    private void OnGUI ()
    {
        DrawWindow();
    }

    private void DrawWindow(){  // 绘制
        UpdateAssetsTreeData(); 
        m_MyAssetTreeView.OnGUI(new Rect(win.x, win.y, position.width, position.height));
    }
    
    /// <summary>
    /// 数据更新
    /// </summary>
    private void UpdateAssetsTreeData(){
        var CurrentPra = AssetDatabase.GetAssetPath(hasTar);
        m_MyAssetTreeView.Root = new MyAssetTreeViewItem(0, -1, "root", false); // root节点
        DrawNodeTree(transforms); // 获取所有节点并组装成树
        GetMissingList(CurrentPra); // 获取丢失组件的节点
        m_MyAssetTreeView.Reload(); // 调用
    }


    private void GetMissingList(string assetPath){
        var i = 0;
        GameObject tempObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        Component[] components = tempObj.GetComponentsInChildren<Component>();
        if(components.Length > 0){
            foreach(var co in components){
                if(co){
                    SerializedObject so = new SerializedObject(co);
                    var iter = so.GetIterator();
                    while(iter.NextVisible(true)){
                        if(iter.propertyType == SerializedPropertyType.ObjectReference){
                            if(iter.objectReferenceValue == null && iter.objectReferenceInstanceIDValue != 0){
                                var trans = co.GetComponent<Transform>();
                                var pid = Array.IndexOf(transforms, trans);
                                var parentItem = m_MyAssetTreeView._FindItem(pid+1, m_MyAssetTreeView.Root);
                                var misItem = new MyAssetTreeViewItem(++currId, parentItem.depth + 1,iter.name, true, AssetDatabase.GetAssetPath(co.gameObject));
                                parentItem.AddChild(misItem);
                            }
                        }
                    }
                }else{  // 脚本组件
                    // 寻找父节点
                    var trans = components[i - 1].GetComponent<Transform>();
                    var parentID = Array.IndexOf(transforms, trans);
                    var parentItem = m_MyAssetTreeView._FindItem(parentID + 1, m_MyAssetTreeView.Root);
                    var MisScriptItem = new MyAssetTreeViewItem(++currId, parentItem.depth + 1, "MissingScript",  true);
                    parentItem.AddChild(MisScriptItem);
                }
                i++;
            }
        }
    }
    /// <summary>
    /// 节点树生成
    /// </summary>
    /// <param name="trans"></param>
    private void DrawNodeTree(Transform[] trans){
        for(int i = 0; i < trans.Length; i++){
            var item = new MyAssetTreeViewItem(i + 1,GetDepth(trans[i], 1),trans[i].name, false);
            // 根据id获得父节点
            if(i == 0){
                item.parent = m_MyAssetTreeView.Root;
            }else{
                var _pid = Array.IndexOf(trans, trans[i].parent);
                item.parent = m_MyAssetTreeView._FindItem(_pid + 1, m_MyAssetTreeView.Root);
                item.SetItemPath(AssetDatabase.GetAssetPath(trans[i].parent.gameObject));
            }
            item.parent.AddChild(item);
            currId = i + 1;
        }
    }

    /// <summary>
    /// 计算节点深度
    /// </summary>
    /// <param name="transform">节点Transform组件</param>
    /// <param name="depth">参数一般默认为1</param>
    /// <returns></returns>
    private int GetDepth(Transform transform, int depth){
        if(transform.parent){
            depth = depth + 1;
            return GetDepth(transform.parent, depth);
        }else{
            return depth;
        }
    }
}
