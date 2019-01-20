using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider)), RequireComponent(typeof(View)), RequireComponent(typeof(KillableBase)), RequireComponent(typeof(AudioSource))]
public class EnemyAI : StateMachineBase, IShootAnimation, IWalkAnimation
{
    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////

    [SerializeField, FoldoutGroup("Attack Properties"), Range(2f,0f)]
    private float accuracy;
    
    [SerializeField, FoldoutGroup("Attack Properties"), Required]
    private ScriptableGun equippedGun;

    [SerializeField, FoldoutGroup("Attack Properties"), Required]
    protected Transform muzzlePointTransform;
    
    [SerializeField, FoldoutGroup("Attack Properties"), Required]
    protected GameObject fireLinePrefab;
    
   // [SerializeField, FoldoutGroup("Pursuit Properties"), SuffixLabel("m", true)]
   // protected float pursuitDistanceThreshold;
    [SerializeField, FoldoutGroup("Pursuit Properties"), SuffixLabel("m/s", true)]
    protected float pursuitRunSpeed;

    
    [FormerlySerializedAs("muzzleAudioSource")] [SerializeField, FoldoutGroup("Audio Properties")]
    protected AudioSource audioSource;

    [SerializeField, FoldoutGroup("Audio Properties"), Required]
    protected WalkSoundScriptable walkingSounds;
    
    
    private List<Transform> activeTargets;
    
    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////
    
    protected string[] AnimationParameters = new string[]
    {
        "Idle",
        "Running",
        "Shooting"
    };

    protected int[] parameterIds;
    
    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////
    
    protected override void Init()
    {
        //Set the Animator parameters to their respective Hash Codes
        parameterIds = new int[AnimationParameters.Length];
        for (int i = 0; i < AnimationParameters.Length; i++)
        {
            parameterIds[i] = Animator.StringToHash(AnimationParameters[i]);
        }

        GetComponent<KillableBase>().onHitCallback += Hit;

        //We assign the audio source, and then make sure that its 3D
        audioSource = gameObject.GetComponent<AudioSource>();
        //audioSource.spatialBlend = 1f;
        //audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        //audioSource.minDistance = 3f;
        //audioSource.maxDistance = 10f;
        //audioSource.spatialize = true;
        
        InitState(STATE.IDLE);
    }

    protected override void InitState(STATE newState, object parameters = null)
    {
        //Sets the gameObject name to better debug in-editor
        
        currentState = newState;
        
        name = string.Format("[{0}]{1}", currentState, startingName);

        //Reset any timer that we have so we dont have any conflicts
        mTimer = 0f;

        switch (currentState)
        {
            case STATE.IDLE:
                break;
            case STATE.WANDER:
                //Pick a point between current location and max view distance
                navMeshAgent.SetDestination((Vector3?) parameters ?? RandomNavmeshLocation(view.MaxViewDistance));
                navMeshAgent.speed = speed;
                break;
            case STATE.PURSUE:
                break;
            case STATE.ATTACK:
                //TODO Instead of B-Lining it to the player, maybe the AI should find some cover
                if (parameters != null)
                {
                    //Move towards the player while shooting
                    var dir = (transform.position - (Vector3?) parameters).Value.normalized;
                    var pos = (Vector3) parameters + dir * (view.MaxViewDistance / 2f);
                    navMeshAgent.SetDestination(pos);
                }
                else
                {
                    navMeshAgent.SetDestination(transform.position);
                }
                navMeshAgent.speed = pursuitRunSpeed;
                break;
            case STATE.DEAD:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //SetAnimationState();
    }

   protected override void SetAnimationState()
   {
       ////[0] Idle
       ////[1] Running
       ////[2] Shooting
       //
       //animator.SetBool(parameterIds[0], false);
       ////animator.SetBool(parameterIds[1], false);
       //animator.SetBool(parameterIds[2], false);
       //
       //switch (currentState)
       //{
       //    case STATE.IDLE:
       //        animator.SetBool(parameterIds[0], true);
       //        break;
       //    case STATE.PURSUE:
       //        //animator.SetBool(parameterIds[1], true);
       //        animator.SetBool(parameterIds[2], true);
       //        break;
       //    case STATE.WANDER:
       //        animator.SetBool(parameterIds[1], true);
       //        break;
       //    case STATE.ATTACK:
       //        animator.SetBool(parameterIds[2], true);
       //        break;
       //    case STATE.DEAD:
       //        break;
       //    default:
       //        throw new ArgumentOutOfRangeException(currentState.ToString());
       //}
   }

    private void LateUpdate()
    {
        view.CanSeeTargets(out activeTargets);
     
        ////[0] Idle
        ////[1] Running
        ////[2] Shooting
        animator.SetBool(parameterIds[0], currentState == STATE.IDLE);
        animator.SetBool(parameterIds[1], navMeshAgent.remainingDistance > 0.5f);
        animator.SetBool(parameterIds[2], currentState == STATE.ATTACK);
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

    protected override void WanderState()
    {
        if (activeTargets != null && activeTargets.Count > 0)
        {
            InitState(STATE.ATTACK, activeTargets[0].position);
            return;
        }

        if (navMeshAgent.remainingDistance < 0.2f)
        {
            InitState(STATE.IDLE);
            return;
        }
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
            InitState(STATE.WANDER, lastTargetPosition );
            return;
        }     

        Vector3 lookAtTarget = lastTargetPosition = activeTargets[0].position;

        if (Vector3.Distance(transform.position, lookAtTarget) > view.MaxViewDistance / 2f)
        {
            InitState(STATE.ATTACK, lookAtTarget);
            return;
        }
        else if (navMeshAgent.remainingDistance < 0.2f)
        {
            //Pick Random location inside of the pursuit threshold
            InitState(STATE.ATTACK, RandomNavmeshLocationAroundPosition(lookAtTarget, view.MaxViewDistance / 2f));
        }
        
        var position = transform.position;
        lookAtTarget.y = position.y;

        transform.forward = (lookAtTarget - position).normalized;

        //TODO Need to chase player, without getting too close
        //TODO Shoot at player
    }

    protected override void DeadState()
    {
        throw new System.NotImplementedException();
    }
    
    #endregion //States

    void Hit(Vector3 fromPosition)
    {
        if (currentState == STATE.ATTACK)
            return;

        InitState(STATE.WANDER, GetNavMeshPointFromPosition(fromPosition));
    }
    
    //Thanks to http://answers.unity.com/answers/1426690/view.html
    protected Vector3 RandomNavmeshLocation(float radius) 
    {
        return RandomNavmeshLocationAroundPosition(transform.position, radius);
    }
    protected Vector3 RandomNavmeshLocationAroundPosition(Vector3 position, float radius) 
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += position;
        NavMeshHit hit;
        Vector3    finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
            finalPosition = hit.position;            
        }
        return finalPosition;
    }

    protected Vector3 GetNavMeshPointFromPosition(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 5f, 1))
            return hit.position;

        return position;
    }

    private void OnDestroy()
    {
        GetComponent<KillableBase>().onHitCallback -= Hit;
    }

    #region Animation Listeners
    
    public void AnimationShoot(int amount)
    {
        Vector3 fireDirection = lastTargetPosition - muzzlePointTransform.position;
        fireDirection += transform.right * Random.Range(-accuracy, accuracy);
        fireDirection += transform.up * Random.Range(-accuracy, accuracy);
        
        equippedGun.Fire(muzzlePointTransform.position, fireDirection, audioSource);
        Debug.DrawRay(muzzlePointTransform.position, fireDirection, Color.red, 2f);

        GameObject trail;

        //Creates a bullet train to add better visuals for where the AI is shooting
        if (!RecycleManager.TryGetItem("BulletTrail", out trail))
        {
            trail = Instantiate(fireLinePrefab);
            
        }
        
        trail.GetComponent<BulletTrail>().Init(muzzlePointTransform.position,fireDirection);
    }
    
    public void WalkAnimation()
    {
        walkingSounds.PlayWalkingAudio(audioSource);
    }
    
    #endregion //Animation Listeners
    
    
    #region Editor Functions

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, navMeshAgent.destination);
    }

    #endregion //Editor Functions

    
}
