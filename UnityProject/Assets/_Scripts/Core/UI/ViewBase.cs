using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEngine;

public class ViewBase : MonoBehaviour, ICompositeDisposable
{
    private bool _isEnter;
    public bool IsEnter => _isEnter;

    private bool _isInitial;
    public bool IsInitial => _isInitial;

    private ITuple _viewParam;
    public ITuple ViewParam => _viewParam;

    private RectTransform _rectTransform;
    public RectTransform RectTransform => _rectTransform ??= GetComponent<RectTransform>();

    private CompositeDisposable _disposable;
    public CompositeDisposable Disposable => _disposable ??= new CompositeDisposable();

    private List<ViewBase> _subViewList = new List<ViewBase>();

    private void Initial()
    {
        if (!_isInitial)
        {
            this.BindComponent();

            OnInitial();

            InitialSubView();

            _isInitial = true;
        }
    }

    private async void Enter()
    {
        if (!_isEnter)
        {
            await OnEnterTween();

            OnEnter();

            EnterSubView();

            _isEnter = true;
        }
    }

    private async UniTask Leave()
    {
        if (_isEnter)
        {
            Dispose();

            LeaveSubView();

            await OnLeaveTween();

            OnLeave();

            _isEnter = false;
        }
    }

    private void LeaveImmediate()
    {
        if (_isEnter)
        {
            Dispose();

            LeaveSubView();

            OnLeave();

            _isEnter = false;
        }
    }

    private void SetParam(ITuple param)
    {
        _viewParam = param;
    }

    public bool IsShow => gameObject.activeSelf;

    public void SetShow(bool show, ITuple param = null, bool immediate = false)
    {
        if (show)
        {
            Show(param);
        }
        else
        {
            Hide(immediate);
        }
    }

    public void Show(ITuple param = null)
    {
        SetParam(param);

        Initial();

        Enter();

        OnRefresh();

        this.SetVisible(true);
    }

    public async void Hide(bool immediate = false)
    {
        if (immediate)
        {
            LeaveImmediate();
        }
        else
        {
            await Leave();
        }

        this.SetVisible(false);
    }

    private void OnDestroy()
    {
        Hide();
    }

    private void Dispose()
    {
        if (_disposable != null)
        {
            _disposable.Dispose();
            _disposable = null;
        }
    }

    public void AddSubView(ViewBase subView)
    {
        _subViewList.Add(subView);
    }

    public void RmoveSubView(ViewBase subView)
    {
        _subViewList.Remove(subView);
    }

    protected virtual void OnInitial() { }

    protected virtual void OnEnter() { }

    protected virtual UniTask OnEnterTween() => default;

    protected virtual void OnRefresh() { }

    protected virtual UniTask OnLeaveTween() => default;

    protected virtual void OnLeave() { }

    private void InitialSubView()
    {
        foreach (var subView in _subViewList)
        {
            subView.Initial();
        }
    }

    private void EnterSubView()
    {
        foreach (var subView in _subViewList)
        {
            if (subView.IsShow)
            {
                subView.Enter();
            }
        }
    }

    private void LeaveSubView()
    {
        foreach (var subView in _subViewList)
        {
            subView.Leave();
        }
    }
}
