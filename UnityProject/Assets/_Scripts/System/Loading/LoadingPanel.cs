using System;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Cysharp.Threading.Tasks;

[ViewConfig("Loading_LoadingPanel", EViewType.Other, EViewLayer.Loading)]
public class LoadingPanel : ViewRoot
{
    [Path]
    private Image _maskImage;
    public Image MaskImage => _maskImage;

    protected override UniTask OnEnterTween()
    {
        base.OnEnterTween();

        _maskImage.fillAmount = 0;
        _maskImage.fillClockwise = true;
        _maskImage.fillOrigin = (int)Image.OriginVertical.Top;
        var tween = _maskImage.DOFillAmount(1, 0.25f);

        return UniTask.WaitUntil(() =>
        {
            return !tween.IsPlaying();
        });
    }

    protected override UniTask OnLeaveTween()
    {
        _maskImage.fillAmount = 1;
        _maskImage.fillClockwise = false;
        _maskImage.fillOrigin = (int)Image.OriginVertical.Bottom;
        var tween = _maskImage.DOFillAmount(0, 0.5f);

        return UniTask.WaitUntil(() =>
        {
            return !tween.IsPlaying();
        });
    }
}
