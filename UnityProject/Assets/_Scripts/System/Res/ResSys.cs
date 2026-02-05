using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class ResSys : SystemBase<ResSys>
{
    public UniTask<GameObject> Instantiate(string key, Transform parent = null)
    {
        return Addressables.InstantiateAsync(key, parent).ToUniTask();
    }

    public UniTask<T> LoadAsset<T>(string key)
    {
        return Addressables.LoadAssetAsync<T>(key).ToUniTask();
    }

    public void ReleaseAsset<T>(T asset)
    {
        Addressables.Release(asset);
    }

    public async UniTask<bool> IsExist(string key)
    {
        var handle = Addressables.LoadResourceLocationsAsync(key);

        // 等待资源定位器加载完成
        await handle.ToUniTask();

        var locations = handle.Result;

        bool exist = handle.Status == AsyncOperationStatus.Succeeded && locations != null && locations.Count > 0;

        Addressables.Release(handle);

        return exist;
    }
}
