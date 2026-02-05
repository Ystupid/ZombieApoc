using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class PathAttributeExtension
{
    private static Dictionary<Type, List<PathAttributeInfo>> _typeCacheMap;

    static PathAttributeExtension()
    {
        _typeCacheMap = new Dictionary<Type, List<PathAttributeInfo>>();
    }

    public static void BindComponent(Component instance)
    {
        var type = instance.GetType();

        var infoList = GetInfoList(type);

        var rootNode = instance.transform;

        foreach (var info in infoList)
        {
            var target = Find(rootNode, info.Attribute.Path);

            if (target == null)
            {
                continue;
            }

            BindComponent(instance, target, info);
        }
    }

    private static void BindComponent(Component instance, GameObject target, PathAttributeInfo info)
    {
        var fieldType = info.Info.FieldType;

        var component = target.GetComponent(fieldType);

        if (component == null && info.Attribute.AutoAdd)
        {
            component = target.AddComponent(fieldType);
        }

        if (component != null)
        {
            info.Info.SetValue(instance, component);
        }
    }

    private static GameObject Find(Transform root, string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return root.gameObject;
        }
        else
        {
            return root.Find(path)?.gameObject;
        }
    }

    private static List<PathAttributeInfo> GetInfoList(Type type)
    {
        if (!_typeCacheMap.TryGetValue(type, out var infoList))
        {
            infoList = RegisterType(type);
        }

        return infoList;
    }

    private static List<PathAttributeInfo> RegisterType(Type type)
    {
        if (type == null)
        {
            return null;
        }

        if (_typeCacheMap.TryGetValue(type, out var infoList))
        {
            return infoList;
        }

        var pathList = new List<PathAttributeInfo>();
        var baseList = RegisterType(type.BaseType);

        if (baseList != null)
        {
            pathList.AddRange(baseList);
        }

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField);

        foreach (var info in fields)
        {
            var pathAttribute = info.GetCustomAttribute<PathAttribute>();

            if (pathAttribute != null)
            {
                pathList.Add(new PathAttributeInfo
                {
                    Info = info,
                    Attribute = pathAttribute,
                });
            }
        }

        _typeCacheMap.Add(type, pathList);

        return pathList;
    }
}
