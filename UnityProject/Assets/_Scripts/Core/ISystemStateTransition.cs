using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public interface ISystemStateTransition
{
    ESystemState From { get; }
    ESystemState To { get; }

    UniTask Enter();

    UniTask Transition();

    UniTask Leave();
}
