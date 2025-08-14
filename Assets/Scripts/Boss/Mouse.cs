using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public StateMachine stateMachine;
    public State normalState;
    public State attackState;

    [Header("攻击相关")]
    public float attackCoolTime;//攻击间隔时间

    private void Start()
    {
        InitState();
    }

    private void LateUpdate()
    {
        stateMachine.CheckChangeState();
    }

    public void InitState()
    {
        stateMachine=new StateMachine();
        normalState=new NormalState(stateMachine,this);
        attackState=new AttackState(stateMachine,this);
        stateMachine.Init(normalState);
    }
}
