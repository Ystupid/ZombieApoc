using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class ViewRoot : ViewBase
{
    private int _viewID;
    public int ViewID => _viewID;

    private ViewConfig _viewConfig;
    public ViewConfig ViewConfig => _viewConfig;

    private ITweenPlayer _tweenPlayer;
    public ITweenPlayer TweenPlayer => _tweenPlayer ?? CreateTweenPlayer();

    protected virtual ITweenPlayer CreateTweenPlayer() => new AnimationPlayer(this);

    public void OnLoaded(int viewID, ViewConfig viewConfig)
    {
        _viewID = viewID;
        _viewConfig = viewConfig;
    }

    protected override UniTask OnEnterTween()
    {
        return TweenPlayer.PlayOpen();
    }

    protected override UniTask OnLeaveTween()
    {
        return TweenPlayer.PlayClose();
    }
}
