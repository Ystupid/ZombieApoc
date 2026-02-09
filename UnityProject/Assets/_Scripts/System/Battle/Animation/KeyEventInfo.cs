using System;
using System.Collections.Generic;

[Serializable]
public struct KeyEventInfo
{
    public EKeyEvent Type;
    public int IntValue;
    public float FloatValue;
    public string StringValue;
}