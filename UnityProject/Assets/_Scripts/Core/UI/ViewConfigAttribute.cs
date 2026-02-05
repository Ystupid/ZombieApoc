using System;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ViewConfigAttribute : Attribute
{
    public ViewConfig Config;

    public ViewConfigAttribute(string key, EViewType type, EViewLayer layer)
    {
        Config.Key = key;
        Config.Type = type;
        Config.Layer = layer;
    }
}

public struct ViewConfig
{
    public string Key { get; set; }

    public EViewType Type { get; set; }

    public EViewLayer Layer { get; set; }
}
