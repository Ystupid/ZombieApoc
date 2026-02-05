using System;
using System.Collections.Generic;

public class BattleEntitySys : SystemBase<BattleEntitySys>
{
    private List<EntityBase> _entityList;
    public List<EntityBase> EntityList => _entityList;

    protected override void OnInitial()
    {
        base.OnInitial();

        _entityList = new List<EntityBase>();
    }

    protected override void OnLeave()
    {
        base.OnLeave();

        _entityList.Clear();
    }

    public void AddEntity(EntityBase entity)
    {
        _entityList.Add(entity);
    }

    public void GetEntitys<T>(List<T> output) where T : EntityBase
    {
        output.Clear();

        foreach (var entity in _entityList)
        {
            if (entity is T outEntity)
            {
                output.Add(outEntity);
            }
        }
    }
}
