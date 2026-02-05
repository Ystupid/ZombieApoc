using System;
using System.Collections.Generic;
using UnityEngine;

public class ActorEntity : StateEntity<EActorState>
{
    [Path("Actor")]
    protected Animator _animator;
    public Animator Animator => _animator;
}
