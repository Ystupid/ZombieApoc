using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class MainMenuToBattleTransition : ISystemStateTransition
{
    public ESystemState From => ESystemState.MainMenu;
    public ESystemState To => ESystemState.Battle;

    public async UniTask Enter()
    {
        LoadingSys.Instance.OpenLoading();

        await UniTask.WaitUntil(() => UIManager.Instance.IsOpen<LoadingPanel>());
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
