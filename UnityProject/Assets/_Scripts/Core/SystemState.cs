using System;
using System.Collections.Generic;

public class SystemState
{
    public ESystemState StateType;

    public Type[] Systems;

    public SystemState[] SubStates;

    public SystemState(ESystemState stateType, Type[] systems, SystemState[] subStates = null)
    {
        StateType = stateType;
        Systems = systems;
        SubStates = subStates;
    }
}
