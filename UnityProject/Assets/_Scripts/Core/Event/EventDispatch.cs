using System;
using System.Collections.Generic;

public class EventDispatch<TEnum> where TEnum : struct
{
    private Dictionary<TEnum, Dictionary<int, Delegate>> _eventMap;

    public EventDispatch()
    {
        _eventMap = new Dictionary<TEnum, Dictionary<int, Delegate>>();
    }

    public void Dispatch(TEnum mainType, int subType)
    {
        if (TryGetEvent(mainType, subType, out var subEvent) && subEvent is Action callBack)
        {
            callBack();
        }
    }
    public void Dispatch<T1>(TEnum mainType, int subType, T1 p1)
    {
        if (TryGetEvent(mainType, subType, out var subEvent) && subEvent is Action<T1> callBack)
        {
            callBack(p1);
        }
    }

    public void Dispatch<T1, T2>(TEnum mainType, int subType, T1 p1, T2 p2)
    {
        if (TryGetEvent(mainType, subType, out var subEvent) && subEvent is Action<T1, T2> callBack)
        {
            callBack(p1, p2);
        }
    }
    public void Dispatch<T1, T2, T3>(TEnum mainType, int subType, T1 p1, T2 p2, T3 p3)
    {
        if (TryGetEvent(mainType, subType, out var subEvent) && subEvent is Action<T1, T2, T3> callBack)
        {
            callBack(p1, p2, p3);
        }
    }

    public void Dispatch<T1, T2, T3, T4>(TEnum mainType, int subType, T1 p1, T2 p2, T3 p3, T4 p4)
    {
        if (TryGetEvent(mainType, subType, out var subEvent) && subEvent is Action<T1, T2, T3, T4> callBack)
        {
            callBack(p1, p2, p3, p4);
        }
    }

    public void Dispatch<T1, T2, T3, T4, T5>(TEnum mainType, int subType, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
    {
        if (TryGetEvent(mainType, subType, out var subEvent) && subEvent is Action<T1, T2, T3, T4, T5> callBack)
        {
            callBack(p1, p2, p3, p4, p5);
        }
    }

    public void Dispatch<T1, T2, T3, T4, T5, T6>(TEnum mainType, int subType, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
    {
        if (TryGetEvent(mainType, subType, out var subEvent) && subEvent is Action<T1, T2, T3, T4, T5, T6> callBack)
        {
            callBack(p1, p2, p3, p4, p5, p6);
        }
    }

    public void Register(TEnum mainType, int subType, Action callback) => InternalRegister(mainType, subType, callback);
    public void Register<T1>(TEnum mainType, int subType, Action<T1> callback) => InternalRegister(mainType, subType, callback);
    public void Register<T1, T2>(TEnum mainType, int subType, Action<T1, T2> callback) => InternalRegister(mainType, subType, callback);
    public void Register<T1, T2, T3>(TEnum mainType, int subType, Action<T1, T2, T3> callback) => InternalRegister(mainType, subType, callback);
    public void Register<T1, T2, T3, T4>(TEnum mainType, int subType, Action<T1, T2, T3, T4> callback) => InternalRegister(mainType, subType, callback);
    public void Register<T1, T2, T3, T4, T5>(TEnum mainType, int subType, Action<T1, T2, T3, T4, T5> callback) => InternalRegister(mainType, subType, callback);
    public void Register<T1, T2, T3, T4, T5, T6>(TEnum mainType, int subType, Action<T1, T2, T3, T4, T5, T6> callback) => InternalRegister(mainType, subType, callback);

    public void UnRegister(TEnum mainType, int subType, Action callback) => InternalUnRegister(mainType, subType, callback);
    public void UnRegister<T1>(TEnum mainType, int subType, Action<T1> callback) => InternalUnRegister(mainType, subType, callback);
    public void UnRegister<T1, T2>(TEnum mainType, int subType, Action<T1, T2> callback) => InternalUnRegister(mainType, subType, callback);
    public void UnRegister<T1, T2, T3>(TEnum mainType, int subType, Action<T1, T2, T3> callback) => InternalUnRegister(mainType, subType, callback);
    public void UnRegister<T1, T2, T3, T4>(TEnum mainType, int subType, Action<T1, T2, T3, T4> callback) => InternalUnRegister(mainType, subType, callback);
    public void UnRegister<T1, T2, T3, T4, T5>(TEnum mainType, int subType, Action<T1, T2, T3, T4, T5> callback) => InternalUnRegister(mainType, subType, callback);
    public void UnRegister<T1, T2, T3, T4, T5, T6>(TEnum mainType, int subType, Action<T1, T2, T3, T4, T5, T6> callback) => InternalUnRegister(mainType, subType, callback);

    private bool TryGetEvent(TEnum mainType, int subType, out Delegate subEvent)
    {
        subEvent = null;

        if (!_eventMap.TryGetValue(mainType, out var subEventMap))
        {
            return false;
        }

        if (!subEventMap.TryGetValue(subType, out subEvent))
        {
            return false;
        }

        return true;
    }

    private void InternalRegister(TEnum mainType, int subType, Delegate callback)
    {
        if (callback == null)
        {
            throw new ArgumentNullException($"回调为空：MainType->{mainType} SubType->{subType} Callback->{nameof(callback)}");
        }

        if (!_eventMap.TryGetValue(mainType, out var subEventMap))
        {
            subEventMap = new Dictionary<int, Delegate>();

            _eventMap.Add(mainType, subEventMap);
        }

        if (!subEventMap.TryGetValue(subType, out var subEvent))
        {
            subEventMap[subType] = callback;
        }
        else
        {
            subEventMap[subType] = Delegate.Combine(subEvent, callback);
        }
    }

    private void InternalUnRegister(TEnum mainType, int subType, Delegate callback)
    {
        if (callback == null)
        {
            throw new ArgumentNullException($"回调为空：MainType->{mainType} SubType->{subType} Callback->{nameof(callback)}");
        }

        if (!_eventMap.TryGetValue(mainType, out var subEventMap))
        {
            throw new InvalidOperationException($"没有注册该事件：MainType->{mainType} SubType->{subType} Callback->{callback}");
        }

        if (!subEventMap.TryGetValue(subType, out var subEvent))
        {
            throw new InvalidOperationException($"没有注册该事件：MainType->{mainType} SubType->{subType} Callback->{callback}");
        }

        callback = Delegate.Remove(subEvent, callback);

        if (callback == null)
        {
            subEventMap.Remove(subType);
        }
        else
        {
            subEventMap[subType] = callback;
        }
    }
}
