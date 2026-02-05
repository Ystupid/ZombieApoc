using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class NoneToBattleTransition : ISystemStateTransition
{
    public ESystemState From => ESystemState.None;
    public ESystemState To => ESystemState.Battle;

    public UniTask Enter()
    {
        return default;
    }

    public UniTask Leave()
    {
        LoadingSys.Instance.CloseLoading();

        return default;
    }

    public async UniTask Transition()
    {
        await SceneSys.Instance.ChangeScene("Scenes_Battle");
    }
}
