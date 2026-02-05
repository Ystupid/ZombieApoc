using System;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.Field)]
public class PathAttribute : Attribute
{
    public string Path;
    public bool AutoAdd;

    public PathAttribute()
    {
        Path = null;
        AutoAdd = true;
    }

    public PathAttribute(string path, bool autoAdd = true)
    {
        Path = path;
        AutoAdd = autoAdd;
    }
}
