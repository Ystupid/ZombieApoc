using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleResSys : SystemBase<BattleResSys>
{
    private Transform _entity;

    private Dictionary<string, BattleResPool> _poolMap;
    private List<UniTask> _taskList;
    private Dictionary<GameObject, BattleResPool> _toPoolMap;

    protected override void OnInitial()
    {
        base.OnInitial();

        _poolMap = new Dictionary<string, BattleResPool>();
        _toPoolMap = new Dictionary<GameObject, BattleResPool>();
    }

    protected override UniTask OnPreload()
    {
        if (_entity == null)
        {
            _entity = new GameObject("BattleResSys").transform;
            _entity.ResetLocal();
            UnityEngine.Object.DontDestroyOnLoad(_entity);
        }

        _taskList = new List<UniTask>();

        AddPool("Bullet_ArrowEntity", 10);

        UniTask.WhenAll(_taskList);

        return base.OnPreload();
    }

    private void AddPool(string key, int count)
    {
        if (!_poolMap.TryGetValue(key, out var pool))
        {
            pool = new BattleResPool(key, _entity);
            _poolMap.Add(key, pool);
        }

        var task = pool.Cache(count);

        _taskList.Add(task);
    }

    public GameObject Spawn(string key)
    {
        if (_poolMap.TryGetValue(key, out var pool))
        {
            var go = pool.Spawn();

            _toPoolMap.Add(go, pool);

            return go;
        }

        return null;
    }

    public void Recycle(GameObject go)
    {
        if (_toPoolMap.TryGetValue(go, out var pool))
        {
            pool.Recycle(go);
            _toPoolMap.Remove(go);
        }
    }
}
