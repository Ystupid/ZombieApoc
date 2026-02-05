using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    private IAstarAI _controller;
    private Vector2 _targetPoint;
    private Animator _animator;
    private BotData _botData;
    private StateMachine<EBotState> _stateMachine;

    private void Awake()
    {
        _controller = GetComponent<IAstarAI>();
        _animator = GetComponentInChildren<Animator>();
        _botData = new BotData();
        _stateMachine = new StateMachine<EBotState>();
        _stateMachine.AddState(EBotState.Idle, new BotIdleState());
        _stateMachine.AddState(EBotState.Move, new BotMoveState());
        _stateMachine.Init(EBotState.Idle);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _targetPoint = CameraSys.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        _controller.destination = _targetPoint;

        var velocity = _controller.velocity;

        var direction = velocity.normalized;

        _animator.SetFloat("VelocityX", direction.x);
        _animator.SetFloat("VelocityY", direction.y);
    }
}
