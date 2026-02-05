using System;
using System.Collections.Generic;

public static class LanguageExtension
{
    public static string Local(this string value)
    {
        var localEndIndex = value.IndexOf(">");

        return value.Substring(localEndIndex + 1);
    }
}
