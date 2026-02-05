using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SystemStateMachine
{
    private ESystemState _currentStateType;
    public ESystemState CurrentStateType => _currentStateType;

    private bool _isSwitching;
    private Queue<ESystemState> _switchQueue;

    private SystemTree _systemTree;
    private List<SystemBase> _systemList;
    private List<SystemBase> _systemUpdateList;
    private Dictionary<Type, SystemBase> _systemMap;
    private Dictionary<ulong, ISystemStateTransition> _transitionMap;
    private Dictionary<ESystemState, SystemState[]> _stateMap;
    private Type _baseDeclaringType;

    public SystemStateMachine(SystemTree tree)
    {
        _systemTree = tree;
        _switchQueue = new Queue<ESystemState>();
        _systemList = new List<SystemBase>();
        _systemUpdateList = new List<SystemBase>();
        _systemMap = new Dictionary<Type, SystemBase>();
        _transitionMap = new Dictionary<ulong, ISystemStateTransition>();
        _stateMap = new Dictionary<ESystemState, SystemState[]>();
        _baseDeclaringType = typeof(SystemBase);
        _stateMap.Add(ESystemState.None, new SystemState[] { });

        InitStateMachine(_systemTree);
    }

    public void Start()
    {
        try
        {
            InitSystem();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    public void Update()
    {
        UpdateState();
    }

    public void Destroy()
    {
        try
        {
            FinalizeSystem();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    private ulong GetTransitionHash(ESystemState from, ESystemState to)
    {
        ulong fromHash = (uint)from.GetHashCode();
        ulong toHash = (uint)to.GetHashCode();

        return fromHash << 32 | toHash;
    }

    public void AddTransition(ISystemStateTransition transition)
    {
        var hash = GetTransitionHash(transition.From, transition.To);

        if (_transitionMap.ContainsKey(hash))
        {
            return;
        }

        _transitionMap[hash] = transition;
    }

    public ISystemStateTransition GetTransition(ESystemState from, ESystemState to)
    {
        var hash = GetTransitionHash(from, to);

        _transitionMap.TryGetValue(hash, out var transition);

        return transition;
    }

    public async void ChangeState(ESystemState stateType)
    {
        _switchQueue.Enqueue(stateType);

        if (!_isSwitching)
        {
            await HandleSwitchQueue();
        }
    }

    public T GetSystem<T>() where T : SystemBase
    {
        _systemMap.TryGetValue(typeof(T), out var system);

        return (T)system;
    }

    private async UniTask HandleSwitchQueue()
    {
        _isSwitching = true;

        while (_switchQueue.Count > 0)
        {
            await UniTask.NextFrame();
            await SwitchToState(_switchQueue.Dequeue());
        }

        _isSwitching = false;
    }

    private async UniTask SwitchToState(ESystemState nextStateType)
    {
        if (_currentStateType == nextStateType)
        {
            return;
        }

        var transition = GetTransition(_currentStateType, nextStateType);
        var oldStates = _stateMap[_currentStateType];
        var newStates = _stateMap[nextStateType];

        _currentStateType = nextStateType;

        // 计算System深度值
        var sameDepth = -1;
        while (sameDepth + 1 < newStates.Length && sameDepth + 1 < oldStates.Length && newStates[sameDepth + 1] == oldStates[sameDepth + 1])
        {
            ++sameDepth;
        }

        try
        {
            if (transition != null)
            {
                await transition.Enter();
            }

            LeaveState(oldStates, sameDepth);

            if (transition != null)
            {
                await transition.Transition();
            }

            await PreloadState(newStates, sameDepth);

            EnterState(newStates, sameDepth);

            if (transition != null)
            {
                await transition.Leave();
            }
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    private void InitSystem()
    {
        foreach (var system in _systemList)
        {
            system.InitialSystem();
        }
    }

    private void EnterState(SystemState[] states, int depth)
    {
        for (int i = depth + 1; i < states.Length; ++i)
        {
            var state = states[i];
            var length = state.Systems == null ? 0 : state.Systems.Length;

            for (int j = 0; j < length; j++)
            {
                if (_systemMap.TryGetValue(state.Systems[j], out var system))
                {
                    system.EnterSystem();
                }
            }
        }
    }

    private async UniTask PreloadState(SystemState[] states, int depth)
    {
        var systemList = new List<SystemBase>();
        var weightTotal = 0;
        var currentWeight = 0;
        var taskCount = 0;

        for (int i = depth + 1; i < states.Length; ++i)
        {
            var state = states[i];
            var length = state.Systems == null ? 0 : state.Systems.Length;

            for (int j = 0; j < length; j++)
            {
                if (_systemMap.TryGetValue(state.Systems[j], out var system))
                {
                    systemList.Add(system);

                    weightTotal += system.SystemWeight;
                }
            }
        }

        for (int i = 0; i < systemList.Count; i++)
        {
            var system = systemList[i];

            taskCount++;
            var task = system.PreloadSystem();

            task.AddCallback(() =>
            {
                taskCount--;
                currentWeight += system.SystemWeight;
            });
        }

        await UniTask.WaitUntil(() => taskCount <= 0);
    }

    private void UpdateState()
    {
        var length = _systemUpdateList.Count;

        for (int i = 0; i < length; i++)
        {
            var system = _systemUpdateList[i];

            if (system.IsEnter)
            {
                system.UpdateSystem();
            }
        }
    }

    private void LeaveState(SystemState[] states, int depth)
    {
        // 离开需要倒序->和栈保持一致
        for (int i = states.Length - 1; i > depth; --i)
        {
            var state = states[i];
            var length = state.Systems == null ? 0 : state.Systems.Length;

            for (int j = 0; j < length; j++)
            {
                if (_systemMap.TryGetValue(state.Systems[j], out var system))
                {
                    system.LeaveSystem();
                }
            }
        }
    }

    private void FinalizeSystem()
    {
        foreach (var system in _systemList)
        {
            system.FinalizeSystem();
        }
    }

    private void InitStateMachine(SystemTree tree)
    {
        BuildStateMapRecursive(tree.Root, new List<SystemState>());

        if (tree.Transitions != null)
        {
            foreach (var transition in tree.Transitions)
            {
                AddTransition(transition);
            }
        }
    }

    private void BuildStateMapRecursive(SystemState state, List<SystemState> parentList)
    {
        if (_stateMap.ContainsKey(state.StateType))
        {
            return;
        }

        parentList.Add(state);
        _stateMap.Add(state.StateType, parentList.ToArray());

        if (state.Systems != null)
        {
            foreach (var type in state.Systems)
            {
                if (!_systemMap.TryGetValue(type, out _))
                {
                    var instance = Activator.CreateInstance(type, true);

                    if (instance is SystemBase system)
                    {
                        _systemMap.Add(type, system);
                        _systemList.Add(system);

                        var methodUpdate = type.GetMethod("OnUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (!(methodUpdate.DeclaringType == _baseDeclaringType))
                        {
                            _systemUpdateList.Add(system);
                        }
                    }
                }
            }
        }

        if (state.SubStates != null)
        {
            foreach (var childState in state.SubStates)
            {
                BuildStateMapRecursive(childState, parentList);
            }
        }

        parentList.RemoveAt(parentList.Count - 1);
    }
}
