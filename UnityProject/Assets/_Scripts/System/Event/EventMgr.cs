using System;
using System.Collections.Generic;

public class EventMgr
{
    private static EventDispatch<EGameEvent> _dispatch;

    static EventMgr()
    {
        _dispatch = new EventDispatch<EGameEvent>();
    }

    public static void DispatchView<ViewEvent>(ViewEvent subType) where ViewEvent : Enum => _dispatch.Dispatch(EGameEvent.ViewEvent, subType.GetHashCode());
    public static void DispatchView<ViewEvent, T1>(ViewEvent subType, T1 p1) where ViewEvent : Enum => _dispatch.Dispatch(EGameEvent.ViewEvent, subType.GetHashCode(), p1);
    public static void DispatchView<ViewEvent, T1, T2>(ViewEvent subType, T1 p1, T2 p2) where ViewEvent : Enum => _dispatch.Dispatch(EGameEvent.ViewEvent, subType.GetHashCode(), p1, p2);
    public static void DispatchView<ViewEvent, T1, T2, T3>(ViewEvent subType, T1 p1, T2 p2, T3 p3) where ViewEvent : Enum => _dispatch.Dispatch(EGameEvent.ViewEvent, subType.GetHashCode(), p1, p2, p3);
    public static void DispatchView<ViewEvent, T1, T2, T3, T4>(ViewEvent subType, T1 p1, T2 p2, T3 p3, T4 p4) where ViewEvent : Enum => _dispatch.Dispatch(EGameEvent.ViewEvent, subType.GetHashCode(), p1, p2, p3, p4);
    public static void DispatchView<ViewEvent, T1, T2, T3, T4, T5>(ViewEvent subType, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) where ViewEvent : Enum => _dispatch.Dispatch(EGameEvent.ViewEvent, subType.GetHashCode(), p1, p2, p3, p4, p5);
    public static void DispatchView<ViewEvent, T1, T2, T3, T4, T5, T6>(ViewEvent subType, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) where ViewEvent : Enum => _dispatch.Dispatch(EGameEvent.ViewEvent, subType.GetHashCode(), p1, p2, p3, p4, p5, p6);

    public static void Dispatch(EGameEvent mainType, int subType) => _dispatch.Dispatch(mainType, subType);
    public static void Dispatch<T1>(EGameEvent mainType, int subType, T1 p1) => _dispatch.Dispatch(mainType, subType, p1);
    public static void Dispatch<T1, T2>(EGameEvent mainType, int subType, T1 p1, T2 p2) => _dispatch.Dispatch(mainType, subType, p1, p2);
    public static void Dispatch<T1, T2, T3>(EGameEvent mainType, int subType, T1 p1, T2 p2, T3 p3) => _dispatch.Dispatch(mainType, subType, p1, p2, p3);
    public static void Dispatch<T1, T2, T3, T4>(EGameEvent mainType, int subType, T1 p1, T2 p2, T3 p3, T4 p4) => _dispatch.Dispatch(mainType, subType, p1, p2, p3, p4);
    public static void Dispatch<T1, T2, T3, T4, T5>(EGameEvent mainType, int subType, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => _dispatch.Dispatch(mainType, subType, p1, p2, p3, p4, p5);
    public static void Dispatch<T1, T2, T3, T4, T5, T6>(EGameEvent mainType, int subType, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => _dispatch.Dispatch(mainType, subType, p1, p2, p3, p4, p5, p6);

    public static void Register(EGameEvent mainType, int subType, Action callback) => _dispatch.Register(mainType, subType, callback);
    public static void Register<T1>(EGameEvent mainType, int subType, Action<T1> callback) => _dispatch.Register(mainType, subType, callback);
    public static void Register<T1, T2>(EGameEvent mainType, int subType, Action<T1, T2> callback) => _dispatch.Register(mainType, subType, callback);
    public static void Register<T1, T2, T3>(EGameEvent mainType, int subType, Action<T1, T2, T3> callback) => _dispatch.Register(mainType, subType, callback);
    public static void Register<T1, T2, T3, T4>(EGameEvent mainType, int subType, Action<T1, T2, T3, T4> callback) => _dispatch.Register(mainType, subType, callback);
    public static void Register<T1, T2, T3, T4, T5>(EGameEvent mainType, int subType, Action<T1, T2, T3, T4, T5> callback) => _dispatch.Register(mainType, subType, callback);
    public static void Register<T1, T2, T3, T4, T5, T6>(EGameEvent mainType, int subType, Action<T1, T2, T3, T4, T5, T6> callback) => _dispatch.Register(mainType, subType, callback);

    public static void UnRegister(EGameEvent mainType, int subType, Action callback) => _dispatch.UnRegister(mainType, subType, callback);
    public static void UnRegister<T1>(EGameEvent mainType, int subType, Action<T1> callback) => _dispatch.UnRegister(mainType, subType, callback);
    public static void UnRegister<T1, T2>(EGameEvent mainType, int subType, Action<T1, T2> callback) => _dispatch.UnRegister(mainType, subType, callback);
    public static void UnRegister<T1, T2, T3>(EGameEvent mainType, int subType, Action<T1, T2, T3> callback) => _dispatch.UnRegister(mainType, subType, callback);
    public static void UnRegister<T1, T2, T3, T4>(EGameEvent mainType, int subType, Action<T1, T2, T3, T4> callback) => _dispatch.UnRegister(mainType, subType, callback);
    public static void UnRegister<T1, T2, T3, T4, T5>(EGameEvent mainType, int subType, Action<T1, T2, T3, T4, T5> callback) => _dispatch.UnRegister(mainType, subType, callback);
    public static void UnRegister<T1, T2, T3, T4, T5, T6>(EGameEvent mainType, int subType, Action<T1, T2, T3, T4, T5, T6> callback) => _dispatch.UnRegister(mainType, subType, callback);
}