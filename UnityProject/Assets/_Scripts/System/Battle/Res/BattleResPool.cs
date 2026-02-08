using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 游戏资源对象池
/// </summary>
public class BattleResPool
{
    private string _key;
    public string Key => _key;

    private Transform _parent;
    public Transform Parent => _parent;

    private Transform _poolEntity;
    public Transform PoolEntity => _poolEntity;

    public Queue<GameObject> _pool;

    public BattleResPool(string key, Transform parent)
    {
        _key = key;
        _parent = parent;
        _pool = new Queue<GameObject>();

        _poolEntity = new GameObject(_key).transform;
        _poolEntity.SetParent(_parent);
        _poolEntity.ResetLocal();
    }

    public async UniTask Cache(int count)
    {
        var taskList = new List<UniTask<GameObject>>();

        for (int i = 0; i < count; i++)
        {
            var task = ResSys.Instance.Instantiate(_key, _poolEntity);

            taskList.Add(task);
        }

        var assetList = await UniTask.WhenAll(taskList);

        foreach (var asset in assetList)
        {
            Recycle(asset);
        }
    }

    public GameObject Spawn()
    {
        var go = _pool.Dequeue();

        go.gameObject.SetActive(true);

        return go;
    }


    public void Recycle(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(_poolEntity);

        _pool.Enqueue(go);
    }
}
