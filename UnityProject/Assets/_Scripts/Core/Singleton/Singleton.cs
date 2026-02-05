using System;
using System.Collections.Generic;

public class Singleton<T> where T : Singleton<T>, new()
{
    private static T _instance;
    public static T Instance
    {
        get => _instance ??= new T();
    }
}
