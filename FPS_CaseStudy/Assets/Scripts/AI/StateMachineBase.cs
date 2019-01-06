using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.AI;
using Object = UnityEngine.Object;


public abstract class StateMachineBase : MonoBehaviour
{
    public enum STATE
    {
        IDLE,
        RUN,
        ATTACK,
        DEAD
    }

    [SerializeField]
    protected STATE currentState = STATE.IDLE;
    
    [SerializeField]
    protected View view;

    [SerializeField, FoldoutGroup("Idle Properties"), SuffixLabel("s", true)]
    protected float idleWaitTime;
    
    [SerializeField, FoldoutGroup("Running Properties"), SuffixLabel("m/s", true)]
    protected float speed;
    [SerializeField, FoldoutGroup("Running Properties")]
    protected Vector3 worldTarget;

    protected float mTimer = 0f;
    
    [SerializeField, Required]
    protected Animator animator;
    
    [SerializeField, Required]
    protected NavMeshAgent navMeshAgent;

    protected string startingName;
    
    protected new Transform transform;
    
    
    
    // Start is called before the first frame update
    private void Start()
    {
        transform = gameObject.transform;
        startingName = gameObject.name;
        
        Init();
    }

    protected abstract void Init();

    protected abstract void InitState(STATE newState, object parameters = null);

    protected abstract void SetAnimationState();

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case STATE.IDLE:
                IdleState();
                break;
            case STATE.RUN:
                RunState();
                break;
            case STATE.ATTACK:
                AttackState();
                break;
            case STATE.DEAD:
                DeadState();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    #region States

    protected abstract void IdleState();
    protected abstract void RunState();
    protected abstract void AttackState();
    protected abstract void DeadState();
    
    #endregion //States
    
}
