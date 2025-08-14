using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    private Mouse mouse;
    public AttackState(StateMachine _stateMachine, Mouse mouse) : base(_stateMachine)
    {
        this.mouse = mouse;
    }
}
