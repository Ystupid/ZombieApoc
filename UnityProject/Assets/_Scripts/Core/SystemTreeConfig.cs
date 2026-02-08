using System;
using System.Collections.Generic;

public class SystemTreeConfig
{
    public static readonly SystemTree GameTree = new SystemTree(
        new SystemState(ESystemState.Root,
            new Type[]
            {
                typeof(UISys),
                typeof(ResSys),
                typeof(CameraSys),
                typeof(SceneSys),
                //typeof(TableSys),
                typeof(LoadingSys),
            },
            new SystemState[]
            {
                new SystemState(ESystemState.MainMenu,
                    new Type[]
                    {
                        typeof(MainMenuSys),
                    }),
                new SystemState(ESystemState.Battle,
                    new Type[]
                    {
                        typeof(BattleEntitySys),
                        typeof(BattleResSys),
                    })
            }),
        new List<ISystemStateTransition>
        {
            new MainMenuToBattleTransition(),
            new BattleToMainMenuTransition(),
            new NoneToBattleTransition(),
        });
}
