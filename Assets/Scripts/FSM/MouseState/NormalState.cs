using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : State
{
    private Mouse mouse;

    public NormalState(StateMachine _stateMachine,Mouse mouse) : base(_stateMachine)
    {
        this.mouse = mouse;
    }
}
