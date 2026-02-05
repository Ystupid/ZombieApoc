using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResManager : Singleton<ResManager>
{
    public UniTask<GameObject> Instantiate(string key,Transform parent)
    {
        return Addressables.InstantiateAsync(key,parent).ToUniTask();
    }
}
