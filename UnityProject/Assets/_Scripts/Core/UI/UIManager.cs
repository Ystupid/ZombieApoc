using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private Transform _rootNode;
    public Transform RootNode => _rootNode;

    private Canvas _mainCanvas;
    public Canvas MainCanvas => _mainCanvas;

    private LinkedList<int> _traceList;
    private Dictionary<int, ViewState> _stateMap;
    private Dictionary<int, ViewConfigAttribute> _configMap;

    public UIManager()
    {
        _configMap = new Dictionary<int, ViewConfigAttribute>();
        _stateMap = new Dictionary<int, ViewState>();
        _traceList = new LinkedList<int>();
    }

    public void Init(GameObject root)
    {
        _rootNode = root.transform;
        _mainCanvas = _rootNode.GetComponentInChildren<Canvas>();
        UnityEngine.Object.DontDestroyOnLoad(_rootNode);
        ViewLayer.Init(_mainCanvas);
    }

    public void Register<T>() => Register(typeof(T));
    public void Register(Type viewType) => Register(viewType.GetHashCode());

    public void Register(int viewID)
    {
        if (!_stateMap.TryGetValue(viewID, out var state))
        {
            state = new ViewState();

            _stateMap[viewID] = state;
        }

        state.IsRegister = true;
    }

    public void IsRegister<T>() => Register(typeof(T));
    public void IsRegister(Type viewType) => IsRegister(viewType.GetHashCode());

    public bool IsRegister(int viewID)
    {
        return _stateMap.TryGetValue(viewID, out var state) && state.IsRegister;
    }

    public void Release<T>() => Release(typeof(T));
    public void Release(Type viewType) => Release(viewType.GetHashCode());

    public void Release(int viewID)
    {
        if (!_stateMap.TryGetValue(viewID, out var state))
        {
            return;
        }

        _stateMap.Remove(viewID);

        if (state.ViewRoot != null)
        {
            state.ViewRoot.Hide(true);
            ResSys.Instance.ReleaseAsset(state.Asset);
        }
    }

    public bool IsOpen<T>() => IsOpen(typeof(T));

    public bool IsOpen(Type viewType) => IsOpen(viewType.GetHashCode());

    public bool IsOpen(int viewID)
    {
        return _stateMap.TryGetValue(viewID, out var state) && state.ViewRoot && state.ViewRoot.IsEnter;
    }

    public void Open<T>(ITuple param = null, ViewConfig config = default) where T : ViewRoot
    {
        OpenAsync<T>(param, config).Forget();
    }

    public UniTask<ViewRoot> OpenAsync<T>(ITuple param = null, ViewConfig config = default) where T : ViewRoot
    {
        return OpenAsync(typeof(T), param, config);
    }

    public void Close<T>() where T : ViewRoot
    {
        Close(typeof(T));
    }

    public void Close(Type viewType)
    {
        var viewID = viewType.GetHashCode();

        RemoveTrace(viewID);

        if (_stateMap.TryGetValue(viewID, out var state))
        {
            SetViewHide(state.ViewRoot, false);
        }
    }

    public UniTask Preload<T>(ViewConfig config = default) => Preload(typeof(T), config);

    public async UniTask Preload(Type viewType, ViewConfig config = default)
    {
        var viewID = viewType.GetHashCode();

        await LoadView(viewType, config);

        var state = _stateMap[viewID];

        if (state.ViewRoot != null)
        {
            SetViewHide(state.ViewRoot, true);
        }
    }

    public async UniTask<ViewRoot> OpenAsync(Type viewType, ITuple param = null, ViewConfig config = default)
    {
        var viewID = viewType.GetHashCode();

        if (!IsRegister(viewID))
        {
            throw new Exception($"请先注册该View->{viewType}");
        }

        var state = _stateMap[viewID];

        if (state.LoadState != EViewLoadState.Finish)
        {
            await LoadView(viewType, config);
        }

        if (state.ViewRoot != null)
        {
            AddTrace(state.ViewRoot);
            SetViewShow(state.ViewRoot, param);
        }

        return state.ViewRoot;
    }

    private async UniTask LoadView(Type viewType, ViewConfig config = default)
    {
        var viewID = viewType.GetHashCode();

        if (!_stateMap.TryGetValue(viewID, out var state))
        {
            _stateMap.Add(viewID, state = new ViewState());
        }

        if (state.LoadState == EViewLoadState.Loading)
        {
            await UniTask.WaitUntil(() => state.LoadState == EViewLoadState.Finish);
        }
        else if (state.LoadState == EViewLoadState.None)
        {
            state.LoadState = EViewLoadState.Loading;

            if (string.IsNullOrEmpty(config.Key))
            {
                config = GetViewConfig(viewType);
            }

            var layer = ViewLayer.GetLayer(config.Layer);

            var asset = await ResSys.Instance.Instantiate(config.Key, layer);

            if (asset == null)
            {
                state.LoadState = EViewLoadState.Failure;
            }
            else
            {
                state.Asset = asset;
                state.ViewRoot = (ViewRoot)asset.AddComponent(viewType);
                state.ViewRoot.OnLoaded(viewID, config);
                state.LoadState = EViewLoadState.Finish;
            }
        }
    }

    private ViewConfig GetViewConfig(Type type)
    {
        var viewID = type.GetHashCode();

        if (!_configMap.TryGetValue(viewID, out var config))
        {
            config = type.GetCustomAttribute<ViewConfigAttribute>();
        }

        return config.Config;
    }

    private void AddTrace(ViewRoot viewRoot)
    {
        var config = GetViewConfig(viewRoot.GetType());
        var viewType = config.Type;
        var viewID = viewRoot.GetType().GetHashCode();

        if (_traceList.Contains(viewID))
        {
            if (_traceList.Last.Value == viewID)
            {
                return;
            }
            else
            {
                RemoveTrace(viewID, false);
            }
        }

        var lastNode = _traceList.Last;

        while (_traceList.Count > 0 && lastNode != null)
        {
            var lastViewID = lastNode.Value;

            if (_stateMap.TryGetValue(lastViewID, out var lastState) && lastState.ViewRoot != null)
            {
                var lastViewType = GetViewConfig(lastState.ViewRoot.GetType()).Type;

                if (lastViewType == EViewType.Popup)
                {
                    SetViewHide(lastState.ViewRoot, true);
                    _traceList.Remove(lastViewID);
                    lastNode = _traceList.Last;
                }
                else if (lastViewType == EViewType.PopupTrace && viewType != EViewType.Popup)
                {
                    SetViewHide(lastState.ViewRoot, true);
                    lastNode = lastNode.Previous;
                }
                else if (lastViewType == EViewType.FullPopup && viewType == EViewType.Normal)
                {
                    SetViewHide(lastState.ViewRoot, true);
                    lastNode = lastNode.Previous;
                }
                else if (lastViewType == EViewType.Normal && viewType == EViewType.Normal)
                {
                    SetViewHide(lastState.ViewRoot, true);
                    break;
                }
                else
                {
                    break;
                }
            }
            else
            {
                _traceList.Remove(lastViewID);
                lastNode = _traceList.Last;
            }
        }

        _traceList.AddLast(viewID);
    }

    private void SetViewShow(ViewRoot viewRoot, ITuple param = null)
    {
        if (viewRoot)
        {
            viewRoot.SetLayer(viewRoot.ViewConfig.Layer);
            viewRoot.Show(param);
        }
    }

    private void SetViewHide(ViewRoot viewRoot, bool immediate)
    {
        if (viewRoot)
        {
            viewRoot.Hide(immediate);
        }
    }

    private void RemoveTrace(int viewID, bool showLastView = true)
    {
        //if (_traceList.Contains(viewID))
        //{
        //    if (_traceList.Last.Value != viewID)
        //    {
        //        var closeTrace = _traceList.Find(viewID);
        //        var nextTrace = closeTrace.Next;

        //        while (nextTrace != null && _stateMap.TryGetValue(nextTrace.Value, out var nextState) && nextState.ViewRoot != null)
        //        {
        //            var viewType = GetViewConfig(nextState.ViewRoot.GetType()).Type;

        //            if (viewType == EViewType.Normal) break;

        //            SetViewHide(nextState.ViewRoot, true);
        //            _traceList.Remove(nextTrace);
        //            nextTrace = nextTrace.Next;
        //        }

        //        _traceList.Remove(closeTrace);

        //        if (!showLastView)
        //        {
        //            return;
        //        }
        //    }

        //    if (_traceList.Contains(viewID))
        //    {
        //        _traceList.Remove(viewID);
        //    }

        //    LinkedListNode<int> lastNode = _traceList.Last;
        //    if (_stateMap.TryGetValue(viewID, out var viewInfo) && viewInfo.ViewRoot != null)
        //    {
        //        //_executeShowViewRoots.Clear();
        //        var viewType = viewInfo.ViewRoot.ViewConfig.Type;
        //        if (viewType == EViewType.Popup || viewType == EViewType.Other) //Popup Other不处理
        //        {
        //            return;
        //        }
        //        else if (viewType == EViewType.PopupTrace ||
        //                 viewType == EViewType.FullPopup) ///关掉的是PopupTrace FullPopup 显示的上一个踪迹
        //        {
        //            if (_traceList.Count > 0 && lastNode != null)
        //            {
        //                var lastViewID = lastNode.Value;
        //                if (_stateMap.TryGetValue(lastViewID, out var lastViewInfo) &&
        //                    lastViewInfo.ViewRoot != null)
        //                {
        //                    SetViewShow(lastViewInfo.ViewRoot, null);
        //                }
        //            }
        //        }
        //        else if (viewType == EViewType.Normal)
        //        {
        //            bool isFindNormalView = false;
        //            while (_traceList.Count > 0 && lastNode != null)
        //            {
        //                var lastVieID = lastNode.Value;

        //                if (_stateMap.TryGetValue(lastVieID, out var lastState) && lastState.ViewRoot != null)
        //                {
        //                    var lastType = GetViewConfig(lastState.ViewRoot.GetType()).Type;

        //                    if (lastType == EViewType.Normal)
        //                    {
        //                        //AddExecuteShowViewRoot(lastViewInfo.ViewEntity);
        //                        lastNode = lastNode.Next;
        //                        isFindNormalView = true;
        //                    }
        //                    else if (isFindNormalView)
        //                    {
        //                        if (lastType != EViewType.FullPopup)
        //                        {
        //                            _traceList.Remove(lastVieID);
        //                        }
        //                        else
        //                        {
        //                            if (lastState.ViewRoot.FullPopupOpenByTrace())
        //                            {
        //                                //AddExecuteShowViewRoot(lastViewInfo.ViewEntity);
        //                            }
        //                            else
        //                            {
        //                                _traceList.Remove(lastVieID);
        //                            }
        //                        }
        //                        lastNode = lastNode.Next;
        //                    }
        //                    else
        //                    {
        //                        lastNode = lastNode.Previous;
        //                    }
        //                }
        //                else
        //                {
        //                    _traceList.Remove(lastVieID);
        //                    lastNode = _traceList.Last;
        //                }

        //            }
        //        }
        //    }
        //}

        //if (_executeShowViewRoots.Count > 0)
        //    _curShowViewRoot = _executeShowViewRoots[_executeShowViewRoots.Count - 1];
    }
}