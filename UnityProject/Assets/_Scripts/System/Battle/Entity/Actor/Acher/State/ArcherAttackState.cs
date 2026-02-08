using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherAttackState : ActorAttackState
{
    private Transform _shootPoint;

    public ArcherAttackState(ActorEntity owner) : base(owner) { }

    public override void OnEnter(ITuple param)
    {
        base.OnEnter(param);

        _shootPoint = Owner.transform.Find("Actor/Anchor/ShootPoint");
        Owner.AddKeyEvent(OnKeyEvent);
    }

    public override void OnLeave()
    {
        base.OnLeave();

        Owner.RemoveKeyEvent(OnKeyEvent);
    }

    private void OnKeyEvent(KeyEventInfo info)
    {
        if (info.Type == EKeyEvent.Shoot)
        {
            OnShoot();
        }
    }

    private void OnShoot()
    {
        var arrow = BattleResSys.Instance.Spawn("Bullet_ArrowEntity");

        var arrowEntity = arrow.gameObject.GetComponentSafe<ArrowEntity>();

        arrowEntity.Init(Owner, _shootPoint);
        arrowEntity.Shoot();
    }
}
