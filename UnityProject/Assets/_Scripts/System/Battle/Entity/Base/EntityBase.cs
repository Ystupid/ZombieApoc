using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase : MonoBehaviour
{
    private bool _isInitial;
    public bool IsInitial => _isInitial;

    private void Awake()
    {
        EntityInit();
    }

    private void Update()
    {
        EntityUpdate();
    }

    private void OnDestroy()
    {
        EntityFinalize();
    }

    public void EntityInit()
    {
        if(!_isInitial)
        {
            this.BindComponent();

            //BattleEntitySys.Instance.AddEntity(this);

            OnEntityInit();

            _isInitial = true;
        }
    }

    public void EntityUpdate()
    {
        OnEntityUpdate();
    }

    public void EntityFinalize()
    {
        OnEntityFinalize();
    }

    protected virtual void OnEntityInit()
    {
    }

    protected virtual void OnEntityUpdate()
    {
    }

    protected virtual void OnEntityFinalize()
    {
    }
}
