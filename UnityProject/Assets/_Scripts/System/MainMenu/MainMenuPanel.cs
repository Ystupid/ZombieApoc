using System;
using System.Collections.Generic;
using UnityEngine.UI;

[ViewConfig("MainMenu_MainMenuPanel", EViewType.Normal, EViewLayer.Normal)]
public class MainMenuPanel : ViewRoot
{
    [Path("StartBtn")]
    private Button _startBtn;

    protected override void OnEnter()
    {
        base.OnEnter();

        _startBtn.onClick.AddListener(OnStartClick);
    }

    protected override void OnLeave()
    {
        base.OnLeave();

        _startBtn.onClick.RemoveListener(OnStartClick);
    }

    private void OnStartClick()
    {
        GameCore.ChangeState(ESystemState.Battle);
    }
}
