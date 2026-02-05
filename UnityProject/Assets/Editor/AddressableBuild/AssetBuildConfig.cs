using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum EBuildStrategy
{
    PackEachFile,       // 指定路径下，每个文件打进一个AB
    PackEachDirectory,  // 指定路径下，每个文件夹打进一个AB
    PaskAll,            // 指定路径下，所有文件打进一个AB
}

public class AssetBuildConfig : ScriptableObject
{
    private const string AssetConfigFilePath = "Assets/AddressableAssetsData/BuildConfig.asset";
    public List<BuildConfig> ConfigList;

    public static AssetBuildConfig GetAssetBuildConfig()
    {
        return AssetDatabase.LoadAssetAtPath<AssetBuildConfig>(AssetConfigFilePath);
    }

    public int GetConfigIndex(string path)
    {
        for (int i = 0; i < ConfigList.Count; i++)
        {
            if (ConfigList[i].BuildPath == path)
                return i;
        }
        return -1;
    }

    public bool HasConfig(string path)
    {
        foreach (var item in ConfigList)
        {
            if (item.BuildPath == path)
                return true;
        }
        return false;
    }

    public void Add(string path)
    {
        BuildConfig config = new BuildConfig()
        {
            BuildPath = path,
            BuildStrategy = EBuildStrategy.PaskAll,
            LimitDepth = 1000,
            IsUseFileNameAsAddress = false,
            ExtensionIgnore = "cs",
            FileIgnoreList = new List<string>(),
            DirIgnoreList = new List<string>(),
        };
        ConfigList.Add(config);
    }

    public void Remove(string path)
    {
        for (int i = ConfigList.Count - 1; i >= 0; i--)
        {
            if (ConfigList[i].BuildPath == path)
                ConfigList.RemoveAt(i);
        }
    }
}

[Serializable]
public struct BuildConfig
{
    public string BuildPath;
    public EBuildStrategy BuildStrategy;

    // 生成AB包时最大的深度
    // 默认是每个目录都会生成一个AB包的，LimitDepth的默认值是 1000
    // 超过了这个目录深度的文件会进入同一个AB包
    // --> 这个名词需要一点理解成本
    public int LimitDepth;

    public bool IsUseFileNameAsAddress;

    public string ExtensionIgnore; // 默认 cs 文件打包的时候一定会忽略

    public List<string> FileIgnoreList;

    public List<string> DirIgnoreList;

    public override string ToString()
    {
        return string.Format($"BuildPath: {BuildPath} BuildStrategy : {BuildStrategy}");
    }
}