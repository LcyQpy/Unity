using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System;
using System.IO;

public class MyAssetTreeView : TreeView
{
    public MyAssetTreeViewItem Root;
    public MyAssetTreeView(TreeViewState treeViewState): base(treeViewState)
    {
        
    }
    protected override TreeViewItem BuildRoot () // 返回根节点
    {
        return Root;
    }
    
    protected override void DoubleClickedItem(int id)
    {
        var item = (MyAssetTreeViewItem)FindItem(id, rootItem);
        //在ProjectWindow中高亮双击资源
        if (item != null)
        {
            var assetObject = AssetDatabase.LoadAssetAtPath(item.GetPath(), typeof(UnityEngine.Object));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = assetObject;
            EditorGUIUtility.PingObject(assetObject);
        } 
    }

    protected override void RowGUI(RowGUIArgs args)
    {
        var item = (MyAssetTreeViewItem) args.item;
        CellGUI(args.rowRect, item, ref args);
        base.RowGUI(args);
    }

    public TreeViewItem _FindItem(int pid, MyAssetTreeViewItem root) // 寻找指定的列表的方法
    {
        return FindItem(pid, root);
    }
    private void CellGUI (Rect cellRect, MyAssetTreeViewItem item, ref RowGUIArgs args){
        if(item.GetMisState()){
            GUI.color = Color.red;  
            GUI.DrawTexture(cellRect, EditorGUIUtility.whiteTexture); // 使用白色纹理填充背景，但颜色由GUI.color决定  
            GUI.color = Color.white; // 恢复默认颜色 
        }
    }
}

public class MyAssetTreeViewItem : TreeViewItem
{
    private string path = "";
    private bool hasMissing = false;
    public MyAssetTreeViewItem(int _id, int _depth, string _name, bool hasMis, string _path = ""){
        id = _id;
        depth = _depth;
        displayName = _name;
        hasMissing = hasMis;
        path = _path;
    }

    public string GetName(){
        return displayName;
    }
    public int GetDepth(){
        return depth;
    }
    public int GetID(){
        return id;
    }
    public bool GetMisState(){
        return hasMissing;
    }
    public string GetPath(){
        return path;
    }
    public void SetItemPath(string _path){
        path = _path;
    }
    
}


