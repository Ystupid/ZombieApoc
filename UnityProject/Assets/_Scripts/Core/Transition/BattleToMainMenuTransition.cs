using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class BattleToMainMenuTransition : ISystemStateTransition
{
    public ESystemState From => ESystemState.Battle;
    public ESystemState To => ESystemState.MainMenu;

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
        await SceneSys.Instance.ChangeScene("Scenes_None");
    }
}
