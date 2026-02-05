using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : ITweenPlayer
{
    private ViewBase _target;
    private Animation _animation;

    public AnimationPlayer(ViewBase target)
    {
        _target = target;
        _animation = _target.GetComponent<Animation>();
    }

    public UniTask PlayOpen()
    {
        if (_animation)
        {
            _animation.Play();
        }
        return default;
    }

    public UniTask PlayClose() => default;
}
