using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

internal class BuildTask
{
    private AssetBuildConfig _assetBuildConfig;
    private BuildConfig _buildConfig;

    private string _buildPath;
    private DirectoryInfo _buildPathDirInfo;

    private List<DirectoryInfo> _taskDirectoriesInfos;
    private List<DirectoryInfo> _taskIgnoreDirectoriesInfos;

    public BuildTask(AssetBuildConfig assetBuildConfig, BuildConfig buildConfig)
    {
        _assetBuildConfig = assetBuildConfig;
        _buildConfig = buildConfig;
    }

    public void UpdateGroupConfig()
    {
        // 记录好这个 task 需要打包的所有目录
        UpdateTaskDirectories();

        // 根据配置的打包策略更新对应的 group 设置
        switch (_buildConfig.BuildStrategy)
        {
            case EBuildStrategy.PackEachFile:
                OneFileOnePack();
                break;

            case EBuildStrategy.PackEachDirectory:
                OneDirectoryOnePack();
                break;

            case EBuildStrategy.PaskAll:
                BuildPathIntoOnlyOnePack();
                break;

            default:
                throw new System.Exception($"配置了一个不支持的打包策略: {_buildConfig.BuildStrategy}");
        }
    }

    // 把 buildtask 需要处理的目录分析出来
    // build config xml 里面有很多个 BuildConfig，打开 xml 文件一看就知道了
    // 每个 BuildConfig 都对应了一个路径
    // 如果有一个 config B 是另外一个 config A 的子路径
    // 那么 config A 在处理的时候会忽视 config B 的路径
    private void UpdateTaskDirectories()
    {
        _buildPath = _buildConfig.BuildPath;
        _buildPathDirInfo = new DirectoryInfo($"Assets/{_buildPath}");

        // 1. 记录需要处理的所有目录
        //    (从根目录开始，把所有子目录也添加进来)
        //    这一段代码运行结束之后，_taskDirectoriesInfos 里就包含了所有要处理的目录了
        _taskDirectoriesInfos = new List<DirectoryInfo>();
        _taskDirectoriesInfos.Add(_buildPathDirInfo);
        AddSubDirectories(_buildPathDirInfo, _taskDirectoriesInfos);

        // 2. 收集需要被 task 忽略的目录
        List<DirectoryInfo> directoriesToIgnoreA = new List<DirectoryInfo>();
        for (int i = 0; i < _assetBuildConfig.ConfigList.Count; i++)
        {
            BuildConfig config = _assetBuildConfig.ConfigList[i];
            DirectoryInfo configDirInfo = new DirectoryInfo($"Assets/{config.BuildPath}");

            // 如果和 task 的 buildPath 是相同的路径，不对这个路径做处理
            bool isSameDir = configDirInfo.FullName == _buildPathDirInfo.FullName;
            if (isSameDir)
                continue;

            // 如果这个路径是 task buildPath 的子目录，那就需要忽略这个路径
            // 这个路径的打包规则会根据 buildConfig 里面的规则去单独进行，当前 task 打包的时候会忽略这个路径
            bool isSubDir = IsSubFolder(configDirInfo.FullName, _buildPathDirInfo.FullName);
            if (isSubDir)
            {
                directoriesToIgnoreA.Add(configDirInfo);
                AddSubDirectories(configDirInfo, directoriesToIgnoreA);
            }
        }

        // 3. directoriesToIgnore 里面包含的目录有以下特征：
        //     - buildConfig 里面设置的目录
        //     - 并且是当前 task 的子目录
        // 这一步处理完之后，_taskDirectoriesInfos 里面存放的就是当前 task 所有需要打包的目录了
        IngoreDirectories(_taskDirectoriesInfos, directoriesToIgnoreA);

        // 4. 把 DirIngoreList 里面的目录排除掉
        List<DirectoryInfo> directoriesToIgnoreB = new List<DirectoryInfo>();
        foreach (var item in _buildConfig.DirIgnoreList)
        {
            if (item.Length <= 2)
                throw new Exception("配置的目录名长度不能短于 2 个英文字符");
            try
            {
                var dirPath = $"Assets/{_buildConfig.BuildPath}/{item}";
                if (!Directory.Exists(dirPath))
                    continue;

                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                directoriesToIgnoreB.Add(dirInfo);
                AddSubDirectories(dirInfo, directoriesToIgnoreB);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"处理目录 Assets/{_buildConfig.BuildPath}/{item} 遇到了异常");
                Debug.LogWarning(e);
            }
        }
        IngoreDirectories(_taskDirectoriesInfos, directoriesToIgnoreB);

        // 把 buildTask 需要 ignore 的这些目录全部存下来, 后面打包文件的时候, 需要忽略这些目录里面的文件
        SetIgnoreDirectores(directoriesToIgnoreA, directoriesToIgnoreB);
    }

    private void SetIgnoreDirectores(List<DirectoryInfo> dirListA, List<DirectoryInfo> dirListB)
    {
        Dictionary<string, DirectoryInfo> uniqueDirectoriesToIgnore = new Dictionary<string, DirectoryInfo>();
        foreach (var item in dirListA)
        {
            if (!uniqueDirectoriesToIgnore.ContainsKey(item.FullName))
                uniqueDirectoriesToIgnore[item.FullName] = item;
        }
        foreach (var item in dirListB)
        {
            if (!uniqueDirectoriesToIgnore.ContainsKey(item.FullName))
                uniqueDirectoriesToIgnore[item.FullName] = item;
        }
        _taskIgnoreDirectoriesInfos = new List<DirectoryInfo>();
        foreach (var item in uniqueDirectoriesToIgnore)
        {
            _taskIgnoreDirectoriesInfos.Add(item.Value);
        }
    }

    private void IngoreDirectories(List<DirectoryInfo> taskDirectoriesInfos, List<DirectoryInfo> directoriesToIgnore)
    {
        // 再次收集一遍 directoriesToIgnores，这一次会对同名的目录做去重的操作
        Dictionary<string, DirectoryInfo> uniqueDirectoriesToIgnore = new Dictionary<string, DirectoryInfo>();
        foreach (var item in directoriesToIgnore)
        {
            if (!uniqueDirectoriesToIgnore.ContainsKey(item.FullName))
                uniqueDirectoriesToIgnore[item.FullName] = item;
        }
        directoriesToIgnore = new List<DirectoryInfo>();
        foreach (var item in uniqueDirectoriesToIgnore)
        {
            directoriesToIgnore.Add(item.Value);
        }

        // 把需要 ignore 的 folders 从列表中排除掉
        for (int i = 0; i < directoriesToIgnore.Count; i++)
        {
            DirectoryInfo ignoreDirectory = directoriesToIgnore[i];
            int index = -1;
            for (int j = 0; j < taskDirectoriesInfos.Count; j++)
            {
                if (ignoreDirectory.FullName == taskDirectoriesInfos[j].FullName)
                {
                    index = j;
                    break;
                }
            }
            if (index >= 0)
                taskDirectoriesInfos.RemoveAt(index);
        }
    }

    private bool IsSubFolder(string subPath, string rootPath)
    {
        if (rootPath.Length >= subPath.Length)
            return false;

        if (subPath.IndexOf(rootPath) == 0)
            return true;

        return false;
    }

    private void AddSubDirectories(DirectoryInfo rootDir, List<DirectoryInfo> taskDirectories)
    {
        DirectoryInfo[] infoList = rootDir.GetDirectories();
        if (infoList != null && infoList.Length > 0)
        {
            for (int i = 0; i < infoList.Length; i++)
            {
                taskDirectories.Add(infoList[i]);
                AddSubDirectories(infoList[i], taskDirectories);
            }
        }
    }

    // "整个路径打入一个AB包"
    // 所有的文件都在一个 AB 包，所有的文件都在一个 group 里面
    // groupName = 路径名字
    // address = 文件名
    // [group - bundle mode] pack together
    private void BuildPathIntoOnlyOnePack()
    {
        PackInOneGroup(BundledAssetGroupSchema.BundlePackingMode.PackTogether);
    }

    // "一个文件一个AB包"
    // 每个文件都有一个 AB 包，所有的文件都在一个 group 里面
    // groupName = 路径名字
    // address = 文件名
    // [group - bundle mode] pack separate
    private void OneFileOnePack()
    {
        PackInOneGroup(BundledAssetGroupSchema.BundlePackingMode.PackSeparately);
    }

    private void PackInOneGroup(BundledAssetGroupSchema.BundlePackingMode filePackingMode)
    {
        string groupName = GetGroupName();

        List<FileInfo> files = new List<FileInfo>();
        foreach (var item in _taskDirectoriesInfos)
        {
            foreach (var file in item.GetFiles())
            {
                files.Add(file);
            }
        }

        // 获取 settings
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
            throw new System.Exception("addressable settings 是空的.");

        // 创建 group 设置
        AddressableAssetGroup group = FindGroup(settings, groupName);
        if (group == null)
        {
            int needadd = 0;
            foreach (var file in files)
            {
                if (file.FullName.EndsWith(".meta"))
                    continue;
                if (file.FullName.EndsWith(".DS_Store"))
                    continue;
                if (IsFileIgnoredByExtension(file.FullName))
                    continue;
                if (IsFileIgnoredByFileName(file.FullName))
                    continue;
                needadd++;
            }
            if (needadd == 0)
            {
                return;
            }
            group = settings.CreateGroup(groupName, false, false, true, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
        }
        BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
        ContentUpdateGroupSchema schema2 = group.GetSchema<ContentUpdateGroupSchema>();
        if (schema == null || schema2 == null)
        {
            Debug.Log($"{group.name}没有找到schema，把group删了重建一遍");
            settings.RemoveGroup(group);
            group = settings.CreateGroup(groupName, false, false, true, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            schema = group.GetSchema<BundledAssetGroupSchema>();

            //throw new Exception($"检查一下这个group的设置，没有schema {group.Name}");
        }
        schema.BundleMode = filePackingMode;

        // 把所有的文件打包放入group
        foreach (var file in files)
        {
            if (file.FullName.EndsWith(".meta"))
                continue;
            if (file.FullName.EndsWith(".DS_Store"))
                continue;
            if (IsFileIgnoredByExtension(file.FullName))
                continue;
            if (IsFileIgnoredByFileName(file.FullName))
                continue;
            string assetPath = AbsolutePathToAssetPath(file.FullName);
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            bool find = settings.FindAssetEntry(guid) != null;
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);

            // 如果已经存在就不要再改名字了
            //if (!find)  //避免错乱的情况名字全部重写一下反正也不花什么时间
            {
                string name = $"{Path.GetFileNameWithoutExtension(file.FullName)}";
                if (filePackingMode == BundledAssetGroupSchema.BundlePackingMode.PackTogether)
                    name = $"{Path.GetFileNameWithoutExtension(file.FullName)}";// _{extension}";
                entry.SetAddress(name);
            }

            int count;
            BuildAddressablesConfig.AddressCount.TryGetValue(entry.address, out count);
            BuildAddressablesConfig.AddressCount[entry.address] = count + 1;
        }
    }

    // "一个目录一个AB包"
    // 一个目录会对应一个 AB 包
    // 每个 AB 包都会对应一个 group，
    // LimitDepth、IsUseFileNameAsAddress 在这个模式会起作用
    // 如果超过了 LimitDepth ，文件打入 group 的时候，会归属到父目录里面去，AB 打包的深度由 LimitDepth 限制，
    // 根目录的 depth == 0，阅读 GetDirDepth() 的代码就能明白这一点了，
    // groupName = 目录对应的路径名字
    // address = 父目录_文件名 或者 文件名 （取决于 IsUseFileNameAsAddress）
    // [group - bundle mode] pack together
    private void OneDirectoryOnePack()
    {
        foreach (var dir in _taskDirectoriesInfos)
        {
            int dirDepth = GetDirDepth(dir);
            if (dirDepth < _buildConfig.LimitDepth)
                BuildDirIntoGroup(dir);
            else if (dirDepth == _buildConfig.LimitDepth)
                BuildDirAndSubDirIntoGroup(dir);
            else
            {
                // 如果目录深度超过了最大深度，就不必做处理了
                // 因为深度超过 limit depth 的文件，全部都进入 limit depth 这个深度的 group 了
            }
        }
    }

    private void BuildDirIntoGroup(DirectoryInfo dirInfo)
    {
        string groupName = GetGroupName(dirInfo);

        // 获取所有的文件
        List<FileInfo> files = new List<FileInfo>();
        foreach (var file in dirInfo.GetFiles())
            files.Add(file);

        BuildFilesIntoGroup(groupName, files);
    }

    private void BuildDirAndSubDirIntoGroup(DirectoryInfo dirInfo)
    {
        string groupName = GetGroupName(dirInfo);

        // 获取所有的文件
        List<FileInfo> files = new List<FileInfo>();
        GetAllFilesInDir(files, dirInfo);

        BuildFilesIntoGroup(groupName, files);
    }

    private void BuildFilesIntoGroup(string groupName, List<FileInfo> files)
    {
        // 获取 settings
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
            throw new System.Exception("addressable settings 是空的.");

        // 创建 group 设置
        AddressableAssetGroup group = FindGroup(settings, groupName);
        if (group == null)
        {
            int needadd = 0;
            foreach (var file in files)
            {
                if (file.FullName.EndsWith(".meta"))
                    continue;
                if (file.FullName.EndsWith(".DS_Store"))
                    continue;
                if (IsFileIgnoredByExtension(file.FullName))
                    continue;
                if (IsFileIgnoredByFileName(file.FullName))
                    continue;
                needadd++;
            }
            if (needadd == 0)
            {
                return;
            }
            group = settings.CreateGroup(groupName, false, false, true, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
        }
        BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
        ContentUpdateGroupSchema schema2 = group.GetSchema<ContentUpdateGroupSchema>();
        if (schema == null || schema2 == null)
        {
            Debug.Log($"{group.name}没有找到schema，把group删了重建一遍");
            settings.RemoveGroup(group);
            group = settings.CreateGroup(groupName, false, false, true, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            schema = group.GetSchema<BundledAssetGroupSchema>();

            //throw new Exception($"检查一下这个group的设置，没有schema {group.Name}");
        }

        schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;

        // 不使用Hash添加到文件名，防止meta中的GUID改变对AssetBundle的名称影响
        schema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;

        // 把所有的文件打包放入group
        foreach (var file in files)
        {
            if (file.FullName.EndsWith(".meta"))
                continue;
            if (file.FullName.EndsWith(".DS_Store"))
                continue;
            if (IsFileIgnoredByExtension(file.FullName))
                continue;
            if (IsFileIgnoredByFileName(file.FullName))
                continue;
            string assetPath = AbsolutePathToAssetPath(file.FullName);
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            bool find = settings.FindAssetEntry(guid) != null;
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);

            //if (!find) // 如果已经存在就不要再改名字了
            {
                if (_buildConfig.IsUseFileNameAsAddress)
                {
                    if (entry != null)
                    {
                        entry.SetAddress($"{Path.GetFileNameWithoutExtension(file.FullName)}");
                    }
                    else
                    {
                        Debug.LogError($"{assetPath}资源有问题");
                    }
                }
                else
                {
                    if (entry == null || file == null || file.Directory == null)
                    {
                        Debug.Log(file.FullName);
                        Debug.Log("test log.");
                    }
                    entry.SetAddress($"{file.Directory.Name}_{Path.GetFileNameWithoutExtension(file.FullName)}");
                }
            }

            int count = 0;
            BuildAddressablesConfig.AddressCount.TryGetValue(entry.address, out count);
            BuildAddressablesConfig.AddressCount[entry.address] = count + 1;
        }
    }

    private AddressableAssetGroup FindGroup(AddressableAssetSettings settings, string groupName)
    {
        if (settings == null || string.IsNullOrEmpty(groupName))
            return null;
        return settings.FindGroup(groupName);
    }

    // 把目录里面所有的file，存在 files 里面, 但是要把 ignore dir 里面的文件排除掉
    private void GetAllFilesInDir(List<FileInfo> files, DirectoryInfo dirInfo)
    {
        foreach (var ignoreDir in _taskIgnoreDirectoriesInfos)
        {
            // 如果这个目录是 ignore dir, 就不取这个目录里面的文件了
            if (ignoreDir.FullName == dirInfo.FullName)
                return;

            // 如果这个目录是 ignore dir 的子目录, 也不能取这个目录里面的文件
            if (IsSubFolder(dirInfo.FullName, ignoreDir.FullName))
                return;
        }

        if (dirInfo.GetFiles() != null)
        {
            foreach (var file in dirInfo.GetFiles())
                files.Add(file);
        }

        if (dirInfo.GetDirectories() != null)
        {
            foreach (var subDir in dirInfo.GetDirectories())
            {
                GetAllFilesInDir(files, subDir);
            }
        }
    }

    // 查询 dirInfo 相对于 _buildPath 的深度，如果和 _buildPath 相同的话，则认为深度是 0
    private int GetDirDepth(DirectoryInfo dirInfo)
    {
        if (dirInfo.FullName == _buildPathDirInfo.FullName)
            return 0;

        if (!IsSubFolder(dirInfo.FullName, _buildPathDirInfo.FullName))
            throw new Exception($"{dirInfo.FullName} 不是 buildPath: {_buildPathDirInfo.FullName} 的子目录，配置文件有问题");

        int depth = 1;
        while (dirInfo.Parent.FullName != _buildPathDirInfo.FullName)
        {
            dirInfo = dirInfo.Parent;
            depth++;
        }

        return depth;
    }

    private string GetGroupName()
    {
        string[] dirList = _buildPath.Split('/');
        string groupName = "";
        for (int i = 0; i < dirList.Length; i++)
        {
            if (i == 0)
                groupName = dirList[i];
            else
                groupName += $"_{dirList[i]}";
        }
        return groupName;
    }

    private string GetGroupName(DirectoryInfo dirInfo)
    {
        string assetPath = AbsolutePathToAssetPath(dirInfo.FullName);
        string[] dirList = assetPath.Split('/');
        string groupName = "";
        for (int i = 1; i < dirList.Length; i++) // 0 号位是 Assets 这个单词，这个会被抛弃掉
        {
            if (i == 1)
                groupName = dirList[i];
            else
                groupName += $"_{dirList[i]}";
        }
        return groupName;
    }

    private string AbsolutePathToAssetPath(string absolutePath)
    {
        string path = absolutePath.Replace("\\", "/");
        path = "Assets" + path.Replace(Application.dataPath, "");
        return path;
    }

    private bool IsFileIgnoredByExtension(string filePath)
    {
        // 这里做个保护，如果不填的话，Split之后的数组只有一个空字符串""，任何string.EndsWith("")都为true
        if (string.IsNullOrEmpty(_buildConfig.ExtensionIgnore))
            return false;

        string[] extensions = _buildConfig.ExtensionIgnore.Split(',');
        foreach (var ext in extensions)
        {
            if (filePath.EndsWith(ext))
                return true;
        }
        return false;
    }

    private bool IsFileIgnoredByFileName(string filePath)
    {
        if (_buildConfig.FileIgnoreList.Count == 0)
            return false;
        int index = Mathf.Max(filePath.LastIndexOf("/"), filePath.LastIndexOf("\\"));
        string fileName = filePath.Substring(index + 1, filePath.Length - index - 1);
        
        foreach (var ignoreFileName in _buildConfig.FileIgnoreList)
        {
            //简单通配符匹配，仅支持以单个通配符开头或结尾的字符串
            if (ignoreFileName.Contains("*"))
            {
                string temp = ignoreFileName.Replace("*", "");
                return fileName.StartsWith(temp) || fileName.EndsWith(temp);
            }
            if (filePath.EndsWith(ignoreFileName))
                return true;
        }
        return false;
    }
}

public static class BuildAddressablesConfig
{
    private const string AddressableDefaultGroupName = "DefaultAssetsGroup";
    private const string AssetConfigFilePath = "BuildAssetConfiguration/BuildAssetConfiguration.xml";
    public static Dictionary<string, int> AddressCount = new Dictionary<string, int>();

    public static void GenerateAddressablesSettingsInBatchMode()
    {
        GenerateAddressablesSettings();
    }

    [MenuItem("游戏工具/Addressable/刷新配置", priority = 1)]
    public static void GenerateAddressablesSettings()
    {
        Debug.Log("开始刷新");
        CheckAndDeleteNullConfigs();//先清一遍把引用丢失和文件名改过的addressable清掉
        RefreshGroupsInfos();
        CheckAndDeleteNullConfigs();
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        Debug.Log("Addressables 信息刷新完毕.");
    }

    public static bool assetBunldeEncryptionEnable = true;

    [MenuItem("Assets/生成目录的Addressables信息")]
    public static void GenerateTargetPathAddressablesSettings()
    {
        var objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        var path = AssetDatabase.GetAssetPath(objs[0]);

        //是文件夹 不是文件夹就不操作了
        if (!string.IsNullOrEmpty(path) && !Path.HasExtension(path))
        {
            //去掉前面的"Assets/"
            string realpath = path.Substring(8);
            Debug.Log("开始刷新");
            CheckAndDeleteNullConfigs();//先清一遍把引用丢失和文件名改过的addressable清掉
            RefreshTargetPathGroupInfo(realpath);
            CheckAndDeleteNullConfigs();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        else
        {
            Debug.Log("选中的不是文件夹");
        }
    }

    // 如果有一些资源被删除了，Addressable Group 里面会显示这个资源 Missing 了
    // 这个函数会清理掉这个 Missing 的资源配置
    private static void CheckAndDeleteNullConfigs()
    {
        Debug.Log("检测并删除 Missing 的资源配置 - start");
        AddressableAssetSettings settings = GetSettings();
        if (settings == null)
            return;
        foreach (var g in settings.groups)
            CheckNullGroup(g);
        RemoveEmptyGroup(settings);
        Debug.Log("检测并删除 Missing 的资源配置 - finish");
    }

    private static void CheckNullGroup(AddressableAssetGroup g)
    {
        if (IsEmptyGroup(g) && !IsDefaultGroup(g))
        {
            Debug.Log($"{g?.Name} is empty.");
        }
        else
        {
            List<AddressableAssetEntry> deleteEntries = new List<AddressableAssetEntry>();
            foreach (var entry in g.entries)
            {
                if (entry == null)
                {
                    Debug.Log($"这里不应该为null的");
                    continue;
                }
                if (string.IsNullOrEmpty(entry.AssetPath))
                {
                    Debug.Log($"{entry.address}的资源是null");
                    if (!File.Exists(entry.AssetPath))
                    {
                        Debug.Log($"{entry.address}的资源的路径文件不存在");
                        deleteEntries.Add(entry);
                    }
                }
            }
            deleteEntries.ForEach(e => g.RemoveAssetEntry(e));
        }
    }

    private static AddressableAssetSettings GetSettings()
    {
        return AddressableAssetSettingsDefaultObject.Settings;
    }

    private static void OnBuildCompleted(AddressableAssetBuildResult res)
    {
        if (!string.IsNullOrEmpty(res.Error))
        {
            throw new Exception("构建失败" + res.Error);
        }
    }

    private static void RefreshGroupsInfos()
    {
        // 1. 初始化 Addressables 的基础设置
        Debug.Log("初始化 Addressables 的基础设置 - start");
        InitAddressableSettings();
        Debug.Log("初始化 Addressables 的基础设置 - end");

        // 2. 加载 AssetBuildConfig
        Debug.Log("加载 AssetBuildConfig - start");
        AssetBuildConfig assetBuildConfig = GetAssetBuildConfig();
        Debug.Log("加载 AssetBuildConfig - end");

        // 3. 刷新 Addressables Groups 的设置
        Debug.Log("刷新 Addressables Groups 的设置 - start");
        UpdateAddressablesGroupsByXmlConfigFile(assetBuildConfig);
        Debug.Log("刷新 Addressables Groups 的设置 - end");
    }

    private static void InitAddressableSettings()
    {
        // 1. 如果 settings 文件不存在的话，创建默认的 Settings 文件
        string folderName = AddressableAssetSettingsDefaultObject.kDefaultConfigFolder;
        string fileName = AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName;
        if (AddressableAssetSettingsDefaultObject.Settings == null)
            AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create(folderName, fileName, false, true);
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        // 2. 创建一个默认的 Group
        string defaultGroupName = AddressableDefaultGroupName;
        AddressableAssetGroup group = settings.FindGroup(defaultGroupName);
        if (group == null)
            settings.CreateGroup(defaultGroupName, true, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
    }

    public static AssetBuildConfig GetAssetBuildConfig()
    {
        // 3. 反序列化 AssetBuildConfig
        AssetBuildConfig assetBuildConfig = AssetBuildConfig.GetAssetBuildConfig();
        if (assetBuildConfig == null)
            throw new System.Exception("加载 Asset 配置文件失败.");
        return assetBuildConfig;
    }

    private static void UpdateAddressablesGroupsByXmlConfigFile(AssetBuildConfig assetBuildConfig)
    {
        // 默认的 settings 如果不存在，就抛一个异常出来
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
            throw new System.Exception("Addressables 没有默认的 Settings，所以没法刷新资源配置.");

        // 如果有重复配置的相同路径，抛出异常报错
        Dictionary<string, int> dirCount = new Dictionary<string, int>();
        foreach (var config in assetBuildConfig.ConfigList)
        {
            if (!dirCount.ContainsKey(config.BuildPath))
                dirCount[config.BuildPath] = 0;
            else
                dirCount[config.BuildPath]++;
        }
        foreach (var item in dirCount)
        {
            if (item.Value > 1)
                throw new System.Exception($"严重错误: 路径 {item.Key} 重复配置了多次");
        }

        // 用来记录 address 的计数，如果有重复的 address 是不允许的，会报错
        AddressCount.Clear();

        // 根据 xml 里面的每一个 build config 来刷新 addressabels group 的配置
        foreach (var config in assetBuildConfig.ConfigList)
        {
            BuildTask buildTask = new BuildTask(assetBuildConfig, config);
            buildTask.UpdateGroupConfig();
        }

        // 清理空的 group
        RemoveEmptyGroup(settings);

        bool hasInvalidKey = false;
        string errorKey = "";
        foreach (var item in AddressCount)
        {
            if (item.Value > 1)
            {
                Debug.LogError($"配置刷新失败，有重复的key: {item.Key}");
                hasInvalidKey = true;
                errorKey = item.Key;
            }
        }

        if (hasInvalidKey)
        {
            // 如果是使用命令行模式, 调用 Unity 刷新 Group 设置
            if (Application.isBatchMode)
                throw new Exception($"生成Addresables失败，重复的 key:{errorKey} 导致配置刷新失败！");
            else
            {
                EditorUtility.DisplayDialog($"Addressables 出错了", $"重复的 key:{errorKey} 导致配置刷新失败, 详情看错误日志。", "确定", "取消");
            }
        }
    }

    // 我们的关卡编辑器会生成很多的 Stage 配置，生成完之后调用这个接口快速刷新 Addressables 配置信息
    public static void GenerateAddressablesSettingsUsedByStages()
    {
        CheckAndDeleteNullConfigs();//先清一遍把引用丢失和文件名改过的addressable清掉
        RefreshGameStageGroupInfo();
        CheckAndDeleteNullConfigs();
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        Debug.Log("关卡的 Addressables 信息刷新完毕.");
    }

    // 这个函数用来快速刷新游戏关卡配置相关的 Addressable Group 配置
    private static void RefreshGameStageGroupInfo()
    {
        // 1. 初始化 Addressables 的基础设置
        InitAddressableSettings();

        // 2. 加载 AssetBuildConfig
        AssetBuildConfig assetBuildConfig = GetAssetBuildConfig();

        // 3. 刷新 Group 的设置
        foreach (var config in assetBuildConfig.ConfigList)
        {
            if (config.BuildPath.Contains("_GameAssets/Stages"))
            {
                BuildTask buildTask = new BuildTask(assetBuildConfig, config);
                buildTask.UpdateGroupConfig();
                Debug.Log("关卡 Addressables 信息更新完毕.");
                return;
            }
        }

        Debug.LogError("生成关卡 Addressables 信息失败，没有找到有效的关卡配置信息.");
    }

    /// <summary>
    /// 这个函数用来快速刷新指定目录的 Addressable Group 配置
    /// </summary>
    /// <param name="path"></param>
    private static void RefreshTargetPathGroupInfo(string path)
    {
        // 1. 初始化 Addressables 的基础设置
        InitAddressableSettings();

        // 2. 加载 AssetBuildConfig
        AssetBuildConfig assetBuildConfig = GetAssetBuildConfig();

        string newpath = path;
        while (newpath.Length > 0)
        {
            Debug.Log(newpath);

            // 3. 刷新 Group 的设置
            foreach (var config in assetBuildConfig.ConfigList)
            {
                if (config.BuildPath.Contains(newpath))
                {
                    BuildTask buildTask = new BuildTask(assetBuildConfig, config);
                    buildTask.UpdateGroupConfig();
                    Debug.Log($"{newpath} Addressables 信息更新完毕.");
                    return;
                }
            }

            //没找到这个路径，找它父级
            int lastindex = newpath.LastIndexOf('/');
            if (lastindex >= 0)
            {
                newpath = newpath.Substring(0, lastindex);
            }
            else
            {
                newpath = "";
            }
        }

        Debug.LogError($"生成{path}的 Addressables 信息失败，没有找到有效的{path}配置信息.");
    }

    private static void RemoveEmptyGroup(AddressableAssetSettings settings)
    {
        if (settings == null)
            return;
        RemoveGroup(settings, g => IsEmptyGroup(g) && !IsDefaultGroup(g));
    }

    private static void RemoveGroup(AddressableAssetSettings settings, Func<AddressableAssetGroup, bool> match)
    {
        if (settings == null || match == null)
            return;

        List<AddressableAssetGroup> delGroups = new List<AddressableAssetGroup>();
        foreach (var g in settings.groups)
        {
            if (match(g))
                delGroups.Add(g);
        }

        foreach (var g in delGroups)
        {
            settings.RemoveGroup(g);
        }
    }

    private static bool IsEmptyGroup(AddressableAssetGroup g)
    {
        return g == null || g.entries == null || g.entries.Count == 0;
    }

    private static bool IsDefaultGroup(AddressableAssetGroup g)
    {
        return g != null && g.Name == AddressableDefaultGroupName;
    }
}