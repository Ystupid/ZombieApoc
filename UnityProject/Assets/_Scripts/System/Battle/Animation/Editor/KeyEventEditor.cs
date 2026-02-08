using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public static class KeyEventEditor
{
    [MenuItem("Assets/Create/KeyEventInfo", false, 100)]
    public static void CreateKeyInfo()
    {
        var info = ScriptableObject.CreateInstance<KeyEventConfig>();

        var path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (string.IsNullOrEmpty(path) || !AssetDatabase.IsValidFolder(path))
        {
            path = "Assets";
        }

        string defaultName = "NewKeyEventInfo";
        string fullPath = $"{path}/{defaultName}.asset";
        int index = 1;

        // 检查文件是否已存在，存在则自动重命名（如 NewCustomConfig (1).asset）
        while (AssetDatabase.LoadAssetAtPath<KeyEventConfig>(fullPath) != null)
        {
            fullPath = $"{path}/{defaultName} ({index}).asset";
            index++;
        }

        // 4. 创建配置文件并导入
        AssetDatabase.CreateAsset(info, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 5. 选中新建的配置文件（符合Unity原生操作习惯）
        Selection.activeObject = info;
        EditorGUIUtility.PingObject(info);

        Debug.Log($"帧事件已创建：{fullPath}");
    }
}
