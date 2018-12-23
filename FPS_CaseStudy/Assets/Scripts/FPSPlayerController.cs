using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public class FPSPlayerController : PlayerController
{
    
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    [SerializeField, FoldoutGroup("Checking Ground")]
    private LayerMask groundLayer;

    [SerializeField,FoldoutGroup("Checking Ground")] 
    private float playerHeight;

    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////// 
    
    [SerializeField,ReadOnly,FoldoutGroup("Information")]
    private bool canMove = true;
    [SerializeField,ReadOnly,FoldoutGroup("Information")]
    private bool isRunning = false;
    [SerializeField,ReadOnly,FoldoutGroup("Information")]
    private bool onGround = true;
    [SerializeField,ReadOnly,FoldoutGroup("Information")]
    private bool ignoreGround = false;
    [SerializeField,FoldoutGroup("Information")]
    private bool isJumping = false;
    
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    
    [SerializeField,FoldoutGroup("Movement")]
    private float moveSpeed = 5f;
    [SerializeField,FoldoutGroup("Movement")]
    private float runSpeed = 7f;

    private Vector3 moveDelta;
    private Vector3 fallDelta;
    
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    
    [SerializeField,FoldoutGroup("Looking"), Required] 
    private Transform playerCamera;
    [SerializeField,FoldoutGroup("Looking")]
    private float lookMultiplier = 1f;
    [SerializeField,FoldoutGroup("Looking")]
    private float maxLookAngle = 90f;

    private Quaternion startCameraRotation;
    
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    
    [SerializeField,FoldoutGroup("Jumping")]
    private float jumpForce = 90f;
    

    private float jumpDelta;

    private new Transform transform;
    private new Rigidbody rigidbody;
    private new Collider collider;
    
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    protected override void Start()
    {
        base.Start();
        
        transform = gameObject.transform;
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        //Init default values
        startCameraRotation = playerCamera.localRotation;
        
        isRunning = false;
        ignoreGround = false;
        canMove = true;
    }

    protected virtual void Update()
    {
        onGround = IsOnGround();
    }

    protected override void InitControls()
    {
        base.InitControls();
        
        test.Gameplay.Sprint.Enable();
        test.Gameplay.Sprint.performed += ctx => { isRunning = ctx.ReadValue<float>() == 1f; };
    }

    private void FixedUpdate()
    {
        //moveDelta = Vector3.zero;
        fallDelta = Vector3.zero;
        
        //FIXME Think that i would prefer to have this in the Normal Update Loop
        ProcessLook();
        ProcessMove();
        
        ProcessFalling();
            


        rigidbody.position += (moveDelta + fallDelta) * Time.fixedDeltaTime;

    }

    protected override void InitState(STATE newState)
    {
        currentState = newState;
        
        switch (newState)
        {
            case STATE.IDLE:
                break;
            case STATE.RUN:
                break;
            case STATE.RELOAD:
                break;
            case STATE.JUMP:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void ProcessStates()
    {
        switch (currentState)
        {
            case STATE.IDLE:
                break;
            case STATE.RUN:
                break;
            case STATE.RELOAD:
                break;
            case STATE.JUMP:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
#region Controls

    protected override void ProcessMove()
    {
        if (canMove == false)
            return;

        if (mMove == Vector2.zero)
        {
            moveDelta = Vector3.zero;
            return;
        }

        var position = rigidbody.position;

        moveDelta = GetDirection() * (isRunning ? runSpeed : moveSpeed);
        
        //rigidbody.MovePosition(Vector3.MoveTowards(
        //    position, 
        //    position + (GetDirection() * 2f),
        //    (isRunning ? runSpeed : moveSpeed) * Time.fixedDeltaTime));
    }

    protected override void ProcessLook()
    {
        if (mLook == Vector2.zero)
            return;
        
        //Used for rotating the player on the transform Y Axis
        Quaternion yRotation = rigidbody.rotation * Quaternion.Euler(0f,lookMultiplier * mLook.x, 0f);
        rigidbody.rotation = yRotation;

        
        //Used for Rotating the player camera on its local X Axis
        Quaternion xRotation = playerCamera.localRotation * Quaternion.Euler(lookMultiplier * mLook.y * -1f,0f, 0f);
        
        if(Quaternion.Angle(startCameraRotation, xRotation)<maxLookAngle) 
            playerCamera.localRotation = xRotation;

    }

    void ProcessFalling()
    {
        var position = rigidbody.position;
        
        fallDelta = Vector3.up * jumpDelta;
        
        //rigidbody.MovePosition(Vector3.MoveTowards(
        //    position, 
        //    position + (Vector3.up * 2f),
        //    jumpDelta * Time.fixedDeltaTime));
        //
        //Assuming that Gravity is always forcing down
        jumpDelta += Physics.gravity.y * Time.fixedDeltaTime;

        if (ignoreGround)
            return;
        
        if (IsOnGround())
        {
            isJumping = false;
            canMove = true;
            jumpDelta = 0f;
        }
    }

    protected override void Jump()
    {
        if (IsOnGround() == false)
            return;
        
        jumpDelta = jumpForce;
        canMove = false;
        isJumping = true;

        StartCoroutine(WaitCoroutine(
            () => { ignoreGround = true; },
            () => { ignoreGround = false; }));
    }

    protected override void Fire()
    {
        throw new System.NotImplementedException();
    }

    protected override void Reload()
    {
        throw new System.NotImplementedException();
    }
    
    #endregion //Controls
    
    #region States

    void MoveState()
    {
        
    }

    void JumpState()
    {
        
    }

    void ReloadState()
    {
        
    }
    
    #endregion //States

    protected override bool IsOnGround()
    {
        var position = transform.position;
        var up = transform.up;
        
        Debug.DrawRay(position, -up.normalized*playerHeight, Color.green);
        
        return Physics.Raycast(position, -up, playerHeight, groundLayer.value);
    }

    protected Vector3 GetDirection()
    {
        Vector3 _out = Vector3.zero;

        _out += transform.forward * mMove.y;
        _out += transform.right * mMove.x;

        return _out.normalized;
    }

    private IEnumerator WaitCoroutine(Action onStart, Action onEnd)
    {
        if (onStart != null)
            onStart();
        
        yield return new WaitForSeconds(0.2f);
        
        if (onEnd != null)
            onEnd();
    }
}
