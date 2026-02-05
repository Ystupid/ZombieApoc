using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class BotState : IState<EBotState>
{
    public EBotState StateType { get; set; }
    public StateMachine<EBotState> Machine { get; set; }

    public virtual void OnEnter(ITuple param) { }

    public virtual void OnInit() { }

    public virtual void OnLeave() { }

    public virtual void OnUpdate() { }
}
