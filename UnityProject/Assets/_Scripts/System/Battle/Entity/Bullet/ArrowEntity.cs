using System;
using System.Collections.Generic;
using UnityEngine;

public class ArrowEntity : EntityBase
{
    private ActorEntity _source;
    private Transform _target;
    private bool _isShoot;

    private Vector2 _startPoint;
    private Vector2 _targetPoint;
    private Vector2 _controlPoint;
    private float _progress;

    private float _releaseTime;
    private bool _enterRelase;

    public void Init(ActorEntity owner, Transform anchor)
    {
        _source = owner;
        _isShoot = false;

        _enterRelase = false;
        _progress = 0;

        transform.SetParent(null);
        transform.position = anchor.transform.position;
        transform.rotation = anchor.rotation;
    }

    public void Shoot(Transform target = null)
    {
        _isShoot = true;
        _target = target;

        if (target == null)
        {
            _targetPoint = _source.transform.position + _source.transform.right * 4f;
            _startPoint = (Vector2)transform.position;

            _controlPoint = (_startPoint + _targetPoint) / 2 + Vector2.up * 2;
        }
    }

    protected override void OnEntityUpdate()
    {
        base.OnEntityUpdate();

        if (_isShoot)
        {
            var position = BezierCurve.GetQuadraticPoint(_startPoint, _controlPoint, _targetPoint, _progress);

            _progress += Time.deltaTime;

            var nextPosition = BezierCurve.GetQuadraticPoint(_startPoint, _controlPoint, _targetPoint, _progress);

            var direction = (nextPosition - position).normalized;

            transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

            transform.position = position;
        }

        if (_progress >= 1)
        {
            _isShoot = false;

            if (!_enterRelase)
            {
                _releaseTime = 2;
                _enterRelase = true;
            }
        }

        if (_enterRelase)
        {
            _releaseTime -= Time.deltaTime;

            if (_releaseTime <= 0)
            {
                BattleResSys.Instance.Recycle(gameObject);
            }
        }
    }
}
