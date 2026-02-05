using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class SystemBase<T> : SystemBase where T : SystemBase<T>
{
    private static T _instance;
    public static T Instance => _instance ??= GameCore.Instance.StateMachine.GetSystem<T>();
}

public class SystemBase : ICompositeDisposable
{
    private CompositeDisposable _disposable;
    public CompositeDisposable Disposable => _disposable ??= new CompositeDisposable();

    public bool IsEnter { get; private set; }

    public int SystemWeight => 1;

    public void InitialSystem() => OnInitial();

    public UniTask PreloadSystem() => OnPreload();

    public void EnterSystem()
    {
        OnEnter();

        IsEnter = true;
    }

    public void UpdateSystem() => OnUpdate();

    public void LeaveSystem()
    {
        if (_disposable != null)
        {
            _disposable.Dispose();
            _disposable = null;
        }

        OnLeave();

        IsEnter = false;
    }

    public void FinalizeSystem() => OnFinalize();

    protected virtual void OnInitial() { }

    protected virtual void OnEnter() { }

    protected virtual UniTask OnPreload() => default;

    protected virtual void OnUpdate() { }

    protected virtual void OnLeave() { }

    protected virtual void OnFinalize() { }
}
