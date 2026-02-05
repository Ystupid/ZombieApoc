using System;
using System.Collections.Generic;

public class LoadingSys : SystemBase<LoadingSys>
{
    protected override void OnEnter()
    {
        base.OnEnter();

        this.BindView<LoadingPanel>();
    }

    public void OpenLoading()
    {
        UIManager.Instance.Open<LoadingPanel>();
    }

    public void CloseLoading()
    {
        UIManager.Instance.Close<LoadingPanel>();
    }
}
