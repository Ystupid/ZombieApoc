using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SystemTree
{
    public SystemState Root;

    public List<ISystemStateTransition> Transitions;

    public SystemTree(SystemState root, List<ISystemStateTransition> transitions)
    {
        Root = root;
        Transitions = transitions;
    }
}
