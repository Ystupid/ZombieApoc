using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;

public static class ICompositeDisposableExtensions
{
    public static void BindView<ViewType>(this ICompositeDisposable disposable)
    {
        Observable.FromEvent(
            register => UIManager.Instance.Register<ViewType>(),
            remove => UIManager.Instance.Release<ViewType>()).Subscribe().AddTo(disposable.Disposable);
    }

    public static void BindPreload<ViewType>(this ICompositeDisposable disposable)
    {
        Observable.FromEvent(
            register => UIManager.Instance.Preload<ViewType>().Forget(),
            remove => UIManager.Instance.Release<ViewType>()).Subscribe().AddTo(disposable.Disposable);
    }

    public static void BindNotice<EventType>(this ICompositeDisposable disposable, EventType eventType, Action callback) where EventType : Enum
    {
        Observable.FromEvent(
            register => EventMgr.Register(EGameEvent.ViewEvent, eventType.GetHashCode(), callback),
            unRegister => EventMgr.UnRegister(EGameEvent.ViewEvent, eventType.GetHashCode(), callback)).Subscribe().AddTo(disposable.Disposable);
    }

    public static void BindNotice<EventType, T1>(this ICompositeDisposable disposable, EventType eventType, Action<T1> callback) where EventType : Enum
    {
        Observable.FromEvent(
            register => EventMgr.Register(EGameEvent.ViewEvent, eventType.GetHashCode(), callback),
            unRegister => EventMgr.UnRegister(EGameEvent.ViewEvent, eventType.GetHashCode(), callback)).Subscribe().AddTo(disposable.Disposable);
    }

    public static void BindNotice<EventType, T1, T2>(this ICompositeDisposable disposable, EventType eventType, Action<T1, T2> callback) where EventType : Enum
    {
        Observable.FromEvent(
            register => EventMgr.Register(EGameEvent.ViewEvent, eventType.GetHashCode(), callback),
            unRegister => EventMgr.UnRegister(EGameEvent.ViewEvent, eventType.GetHashCode(), callback)).Subscribe().AddTo(disposable.Disposable);
    }

    public static void BindNotice<EventType, T1, T2, T3>(this ICompositeDisposable disposable, EventType eventType, Action<T1, T2, T3> callback) where EventType : Enum
    {
        Observable.FromEvent(
            register => EventMgr.Register(EGameEvent.ViewEvent, eventType.GetHashCode(), callback),
            unRegister => EventMgr.UnRegister(EGameEvent.ViewEvent, eventType.GetHashCode(), callback)).Subscribe().AddTo(disposable.Disposable);
    }

    public static void BindNotice<EventType, T1, T2, T3, T4>(this ICompositeDisposable disposable, EventType eventType, Action<T1, T2, T3, T4> callback) where EventType : Enum
    {
        Observable.FromEvent(
            register => EventMgr.Register(EGameEvent.ViewEvent, eventType.GetHashCode(), callback),
            unRegister => EventMgr.UnRegister(EGameEvent.ViewEvent, eventType.GetHashCode(), callback)).Subscribe().AddTo(disposable.Disposable);
    }

    public static void BindNotice<EventType, T1, T2, T3, T4, T5>(this ICompositeDisposable disposable, EventType eventType, Action<T1, T2, T3, T4, T5> callback) where EventType : Enum
    {
        Observable.FromEvent(
            register => EventMgr.Register(EGameEvent.ViewEvent, eventType.GetHashCode(), callback),
            unRegister => EventMgr.UnRegister(EGameEvent.ViewEvent, eventType.GetHashCode(), callback)).Subscribe().AddTo(disposable.Disposable);
    }

    public static void BindNotice<EventType, T1, T2, T3, T4, T5, T6>(this ICompositeDisposable disposable, EventType eventType, Action<T1, T2, T3, T4, T5, T6> callback) where EventType : Enum
    {
        Observable.FromEvent(
            register => EventMgr.Register(EGameEvent.ViewEvent, eventType.GetHashCode(), callback),
            unRegister => EventMgr.UnRegister(EGameEvent.ViewEvent, eventType.GetHashCode(), callback)).Subscribe().AddTo(disposable.Disposable);
    }
}
