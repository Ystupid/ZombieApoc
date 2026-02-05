using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeEntity : StateEntity<EBotState>
{
    [Path("Avatar")]
    private Animator _animator;

    [Path("Avatar")]
    private SpriteRenderer _renderer;

    protected override void InitState()
    {
        base.InitState();

        StateMachine.AddState(EBotState.Idle, new BotIdleState());
        StateMachine.AddState(EBotState.Move, new BotMoveState());
        StateMachine.Init(EBotState.Idle);
    }

    protected override void OnEntityUpdate()
    {
        base.OnEntityUpdate();

        //UpdateAnimation(dir);
    }

    private float _currentAngle;

    private void UpdateAnimation(Vector2 direction)
    {
        var idle = Animator.StringToHash("Idle");
        var moveUp = Animator.StringToHash("Move_Up");
        var moveDown = Animator.StringToHash("Move_Down");
        var moveRight = Animator.StringToHash("Move_Right");
        var moveLeft = Animator.StringToHash("Move_Left");

        var s = Vector2.SignedAngle(Vector2.up, direction);
        float sigAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        var angle = sigAngle < 0 ? 360 + sigAngle : sigAngle;

        _currentAngle = angle;//Mathf.Lerp(_currentAngle, angle, Time.deltaTime * RotateV);

        var anim = idle;

        if (direction == Vector2.zero)
        {
            anim = idle;
        }
        else if (_currentAngle >= 310 || _currentAngle <= 50)
        {
            anim = moveUp;
        }
        else if (_currentAngle >= 50 && _currentAngle <= 130)
        {
            anim = moveRight;
        }
        else if (_currentAngle >= 130 && _currentAngle <= 230)
        {
            anim = moveDown;
        }
        else if (_currentAngle >= 230 && _currentAngle <= 310)
        {
            anim = moveRight;
        }

        _animator.Play(anim);

        _renderer.flipX = _currentAngle >= 180 && _currentAngle <= 360;

        Debug.Log($"S->{s} S2->{sigAngle} Angle->{angle}");
    }
}
