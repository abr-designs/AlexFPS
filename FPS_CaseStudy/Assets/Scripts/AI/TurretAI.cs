using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(View)), RequireComponent(typeof(KillableBase)), RequireComponent(typeof(AudioSource))]
public class TurretAI : StateMachineBase, IRespawnable
{

    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////

    [SerializeField, Required, FoldoutGroup("Turret Properties")]
    private Transform headTransform;
    [SerializeField, FoldoutGroup("Turret Properties")]
    private float wanderTime;
    [SerializeField, FoldoutGroup("Turret Properties")]
    private Vector3 lookMinEuler, lookMaxEuler;
    private Quaternion lookMinRotation, lookMaxRotaion;
    private bool flipRotation;
    
    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////

    [SerializeField, FoldoutGroup("Attack Properties")]
    private float attackCooldown;
    
    [SerializeField, FoldoutGroup("Attack Properties"), Range(2f, 0f)]
    private float accuracy;

    [SerializeField, FoldoutGroup("Attack Properties"), Required]
    private ScriptableGun equippedGun;

    [SerializeField, FoldoutGroup("Attack Properties"), Required]
    protected Transform muzzlePointTransform;

    [SerializeField, FoldoutGroup("Attack Properties"), Required]
    protected GameObject fireLinePrefab;


    [FormerlySerializedAs("muzzleAudioSource")] [SerializeField, FoldoutGroup("Audio Properties")]
    protected AudioSource audioSource;

    protected KillableBase killableBase;

    private List<Transform> activeTargets;

    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////

   //protected string[] AnimationParameters = new string[]
   //{
   //    "Idle",
   //    "Shooting"
   //};

   //protected int[] parameterIds;

    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////

    protected override void Init()
    {
        //Set the Animator parameters to their respective Hash Codes
        //parameterIds = new int[AnimationParameters.Length];
        //for (int i = 0; i < AnimationParameters.Length; i++)
        //{
        //    parameterIds[i] = Animator.StringToHash(AnimationParameters[i]);
        //}

        killableBase               =  GetComponent<KillableBase>();
        killableBase.onHitCallback += Hit;

        //We assign the audio source, and then make sure that its 3D
        audioSource = gameObject.GetComponent<AudioSource>();
        
        lookMinRotation = Quaternion.Euler(lookMinEuler);
        lookMaxRotaion = Quaternion.Euler(lookMaxEuler);

        InitState(STATE.IDLE);
    }

    protected override void InitState(STATE newState, object parameters = null)
    {
        //Sets the gameObject name to better debug in-editor

        currentState = newState;

        name = $"[{currentState}]{startingName}";

        //Reset any timer that we have so we dont have any conflicts
        mTimer = 0f;

        switch (currentState)
        {
            case STATE.IDLE:
                break;
            case STATE.WANDER:
                _t = 0f;
                flipRotation = !flipRotation;
                
                
                break;
            case STATE.PURSUE:
                break;
            case STATE.ATTACK:
                break;
            case STATE.DEAD:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //SetAnimationState();
    }

    [Obsolete]
    protected override void SetAnimationState()
    {

    }

    private void LateUpdate()
    {
        view.CanSeeTargets(out activeTargets);

        if (currentState == STATE.ATTACK)
        {
            animator.speed = 1;
            animator.StartPlayback();
        }
        else
        {
            animator.speed = 0f;
            animator.StopPlayback();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////

    #region States

    protected override void IdleState()
    {
        if (mTimer >= idleWaitTime)
        {
            //TODO Need to move to a new run state
            InitState(STATE.WANDER);
        }
        else
        {
            mTimer += Time.deltaTime;
        }
    }

    [SerializeField, ReadOnly]
    private float _t = 0;
    protected override void WanderState()
    {
        if (activeTargets != null && activeTargets.Count > 0)
        {
            InitState(STATE.ATTACK);
            return;
        }

        if (_t >= 1f)
        {
            InitState(STATE.IDLE);
            return;
        }


        headTransform.localRotation = flipRotation
            ? Quaternion.Lerp(lookMinRotation, lookMaxRotaion, _t += Time.deltaTime)
            : Quaternion.Lerp(lookMaxRotaion, lookMinRotation, _t += Time.deltaTime);
    }

    protected override void PursueState()
    {

    }

    protected override void AttackState()
    {
        //TODO I want to implement some sort of chasing of the player here, so that the AI is chasing the player
        //Its important to not have the AI chase the player, so that he wouldn't be able to get away

        if (activeTargets == null || activeTargets.Count == 0)
        {
            InitState(STATE.WANDER);
            return;
        }

        lastTargetPosition = activeTargets[0].position;
        //Always look at the player in attack mode
        //TODO I should be rotating to face this direction, instead of snapping to it
        headTransform.forward = (lastTargetPosition - headTransform.position).normalized;

        if (mTimer >= attackCooldown)
        {
            mTimer = 0f;
            //TODO Call Attack Here
            Shoot(1);
        }
        else
        {
            mTimer += Time.deltaTime;
        }
        
    }

    protected override void DeadState()
    {


    }

    #endregion //States

    void Hit(Vector3 fromPosition)
    {
        if (killableBase.Health <= 0)
        {
            InitState(STATE.DEAD);
            return;
        }

        if (currentState == STATE.ATTACK)
            return;

        InitState(STATE.WANDER);
    }

    private void OnDestroy()
    {
        killableBase.onHitCallback -= Hit;
    }

    #region Animation Listeners

    private void Shoot(int amount)
    {
        Vector3 fireDirection = lastTargetPosition - muzzlePointTransform.position;
        fireDirection += headTransform.right * Random.Range(-accuracy, accuracy);
        fireDirection += headTransform.up * Random.Range(-accuracy, accuracy);

        equippedGun.Fire(muzzlePointTransform.position, fireDirection, audioSource);
        Debug.DrawRay(muzzlePointTransform.position, fireDirection, Color.red, 2f);

        GameObject trail;

        //Creates a bullet train to add better visuals for where the AI is shooting
        if (!RecycleManager.TryGetItem("BulletTrail", out trail))
        {
            trail = Instantiate(fireLinePrefab);

        }

        trail.GetComponent<BulletTrail>().Init(muzzlePointTransform.position, fireDirection);
    }

    #endregion //Animation Listeners


    #region Editor Functions

    #endregion //Editor Functions

    #region Respawn & Despawn

    public void OnDespawn()
    {

    }

    public void OnRespawn()
    {
        InitState(STATE.IDLE);
    }

    #endregion Respawn & Despawn
}
