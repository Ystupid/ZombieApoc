using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimHash
{
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int Move = Animator.StringToHash("Move");
    public static readonly int Attack = Animator.StringToHash("Attack");
}
