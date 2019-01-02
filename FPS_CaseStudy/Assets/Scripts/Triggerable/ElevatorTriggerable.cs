using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

//FIXME Need to ensure that if the player is under the elevator, they aren't pushed below the level
[RequireComponent(typeof(Rigidbody))]
public class ElevatorTriggerable : TriggerableBase
{
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    
    [SerializeField, BoxGroup("Elevator Information"), PropertyTooltip("Start Position of Elevator in World Space")]
    private Vector3 startPosition;
    [SerializeField, BoxGroup("Elevator Information"), PropertyTooltip("Start Position of Elevator in World Space")]
    private Vector3 endPosition;
    
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////

    [Space, SerializeField, BoxGroup("Elevator Information"), SuffixLabel("m/s", true)]
    private float moveSpeed;
    [SerializeField, BoxGroup("Elevator Information"), SuffixLabel("sec", true)]
    private float endPause;
    
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////

    [Space, SerializeField, BoxGroup("Elevator Information")]
    private AnimationCurve moveCurve = new AnimationCurve();

    [Space, SerializeField, BoxGroup("Elevator Information"), ReadOnly]
    private bool triggered = false;
    
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    
    protected new Rigidbody rigidbody;
    
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////

    
    // Start is called before the first frame update
    protected override void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        
        if(rigidbody.isKinematic == false)
            throw new InvalidOperationException("Rigidbody must be Kinematic");

        rigidbody.position = startPosition;
    }
    

    protected override void Triggered(GameObject other)
    {
        if (triggered)
            return;

        triggered = true;

        StartCoroutine(MoveElevatorCoroutine(() => { triggered = false; }));

    }

    Vector3 targetPosition;
    private void FixedUpdate()
    {
        if(triggered)
            rigidbody.MovePosition(targetPosition);
            
    }

    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////

    [SerializeField, ReadOnly] private float _t = 0f;
    protected IEnumerator MoveElevatorCoroutine(Action onFinished)
    {
        targetPosition = startPosition;
        //float _t = 0f;
        _t = 0f;

        while (_t < 1f)
        {
            targetPosition = Vector3.Lerp(startPosition, endPosition, moveCurve.Evaluate(_t += Time.fixedDeltaTime));
            
           //rigidbody.MovePosition(targetPosition);
            
            yield return new WaitForFixedUpdate();
        }
        
        yield return new WaitForSeconds(endPause);
        
        _t = 0f;
        
        while (_t < 1f)
        {
            targetPosition = Vector3.Lerp(endPosition, startPosition, moveCurve.Evaluate(_t += Time.fixedDeltaTime));
            
            //rigidbody.MovePosition(targetPosition);
            //rigidbody.position = targetPosition;
            
            yield return new WaitForFixedUpdate();
        }

        if (onFinished != null)
            onFinished();

    }
    
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    
    
    #if UNITY_EDITOR
    
    [Button(ButtonSizes.Medium,Name = "Set Start"),HorizontalGroup("Elevator Information/buttons")]
    private void SetStartPosition()
    {
        if (transform == null)
            transform = gameObject.transform;
        
        startPosition = transform.position;
    }

    [Button(ButtonSizes.Medium,Name = "Set End"),HorizontalGroup("Elevator Information/buttons")]
    private void SetEndPosition()
    {
        if (transform == null)
            transform = gameObject.transform;
        
        endPosition = transform.position;
    }

    [Button(ButtonSizes.Medium,Name = "Reset Pos"),HorizontalGroup("Elevator Information/buttons")]
    private void SetPositionToStart()
    {
        if (transform == null)
            transform = gameObject.transform;
        
        transform.position = startPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPosition, 0.25f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(startPosition, endPosition);
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPosition, 0.25f);
    }

#endif
}
