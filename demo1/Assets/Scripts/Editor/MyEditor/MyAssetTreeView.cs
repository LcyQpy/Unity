using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class MyAssetTreeView : TreeView
{
    public MyAssetTreeViewItem Root;
    public List<TreeViewItem> AllItems;
    public MyAssetTreeView(TreeViewState treeViewState): base(treeViewState)
    {
        
    }
    protected override TreeViewItem BuildRoot ()
    {
        return Root;
    }
    public MyAssetTreeView(TreeViewState state,MultiColumnHeader multicolumnHeader):base(state,multicolumnHeader)
    {
        columnIndexForTreeFoldouts = 0;
        showAlternatingRowBackgrounds = true;
        showBorder = false;
    }

    protected override void DoubleClickedItem(int id)
    {
        var item = (MyAssetTreeViewItem)FindItem(id, rootItem);
        //在ProjectWindow中高亮双击资源
        if (item != null)
        {
            var assetObject = AssetDatabase.LoadAssetAtPath(item.path, typeof(UnityEngine.Object));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = assetObject;
            EditorGUIUtility.PingObject(assetObject);
        }
    }

}

public class MyAssetTreeViewItem : TreeViewItem{
   public string path = "";
}




