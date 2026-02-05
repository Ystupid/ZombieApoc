using System;
using System.Collections.Generic;

public class MainMenuSys : SystemBase<MainMenuSys>
{
    protected override void OnEnter()
    {
        base.OnEnter();

        this.BindView<MainMenuPanel>();

        UIManager.Instance.Open<MainMenuPanel>();
    }
}
