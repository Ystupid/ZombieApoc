using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WarriorAttackState : ActorAttackState
{
    private Collider2DTrigger _hitBox;
    private bool _comboCheck;
    private int _nextAnim;
    private bool _needCombo;

    private int _currentAnim;

    public WarriorAttackState(ActorEntity owner) : base(owner) { }

    public override void OnEnter(ITuple param)
    {
        base.OnEnter(param);

        _currentAnim = AnimHash.Attack;
        Owner.AddKeyEvent(OnKeyEvent);
    }

    public override void OnUpdate()
    {
        if (_comboCheck)
        {
            if (AttackTriggerCfg.HasInput(Owner))
            {
                _needCombo = true;
            }
        }

        if (AttackTriggerCfg.NeedCutout(Owner, _currentAnim))
        {
            if (_needCombo)
            {
                _currentAnim = _nextAnim;
                Owner.Animator.CrossFade(_currentAnim, 0.1f);
                _needCombo = false;
            }
            else
            {
                Owner.ChangeState(EActorState.Idle);
            }

            return;
        }
    }

    public override void OnLeave()
    {
        base.OnLeave();

        Owner.RemoveKeyEvent(OnKeyEvent);

        UnBindHitBox(string.Empty);
    }

    private void OnKeyEvent(KeyEventInfo info)
    {
        if (info.Type == EKeyEvent.HitBoxBegin)
        {
            BindHitBox(info.StringValue);
        }
        else if (info.Type == EKeyEvent.HitBoxEnd)
        {
            UnBindHitBox(info.StringValue);
        }
        else if (info.Type == EKeyEvent.ComboBegin)
        {
            _comboCheck = true;
            _nextAnim = Animator.StringToHash(info.StringValue);
        }
        else if (info.Type == EKeyEvent.ComboEnd)
        {
            _comboCheck = false;
            _nextAnim = -1;
        }
    }

    private void OnBoxLeave(Collider2D collider)
    {
        Debug.Log($"OnBoxLeave->{collider}");
    }

    private void OnBoxEnter(Collider2D collider)
    {
        Debug.Log($"OnBoxEnter->{collider}");

        var entity = collider.gameObject.GetComponentSafe<ActorEntity>();

        entity.ChangeState(EActorState.Hurt);
    }

    private void ChangeStep(int step)
    {
    }

    private void BindHitBox(string path)
    {
        _hitBox = Owner.transform.Find(path).GetComponentSafe<Collider2DTrigger>();

        _hitBox.OnEnter.AddListener(OnBoxEnter);
        _hitBox.OnLeave.AddListener(OnBoxLeave);

        _hitBox.gameObject.SetActive(true);
    }

    private void UnBindHitBox(string path)
    {
        if (_hitBox != null)
        {
            _hitBox.OnEnter.RemoveListener(OnBoxEnter);
            _hitBox.OnLeave.RemoveListener(OnBoxLeave);

            _hitBox.gameObject.SetActive(false);
        }

        _hitBox = null;
    }
}
