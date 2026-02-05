using System;
using System.Collections;
using System.Collections.Generic;

public static class ListExtensions
{
    public static bool IsOutRange(this IList list, int index)
    {
        if (list == null)
        {
            return true;
        }

        return index < 0 || index >= list.Count;
    }
}
