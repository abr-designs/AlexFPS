using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public class FPSPlayerController : PlayerController
{
    [SerializeField] private Transform playerCamera;
    
    [SerializeField, FoldoutGroup("Checking Ground")]
    private LayerMask groundLayer;

    [SerializeField,FoldoutGroup("Checking Ground")] 
    private float playerHeight;

    [SerializeField,ReadOnly,FoldoutGroup("Information")]
    private bool canMove = true;
    [SerializeField,ReadOnly,FoldoutGroup("Information")]
    private bool isRunning = false;
    
    [SerializeField,FoldoutGroup("Movement")]
    private float moveSpeed = 5f;
    [SerializeField,FoldoutGroup("Movement")]
    private float runSpeed = 7f;
    
    [SerializeField,FoldoutGroup("Looking")]
    private float lookMultiplier = 1f;
    [SerializeField,FoldoutGroup("Looking")]
    private float maxLookAngle = 90f;

    private Quaternion startCameraRotation;

    private new Transform transform;
    private new Rigidbody rigidbody;
    private new Collider collider;
    

    protected override void Start()
    {
        base.Start();
        
        transform = gameObject.transform;
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        startCameraRotation = playerCamera.localRotation;
        isRunning = false;
    }

    protected virtual void Update()
    {
        
    }

    protected override void InitControls()
    {
        base.InitControls();
        
        test.Gameplay.Sprint.Enable();
        test.Gameplay.Sprint.performed += ctx => { isRunning = ctx.ReadValue<float>() == 1f; };
    }

    private void FixedUpdate()
    {
        
        //FIXME Think that i would prefer to have this in the Normal Update Loop
        ProcessLook();
        ProcessMove();
        
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
            return;

        var position = rigidbody.position;
        
        rigidbody.MovePosition(Vector3.MoveTowards(
            position, 
            position + (GetDirection() * 2f),
            (isRunning ? runSpeed : moveSpeed) * Time.fixedDeltaTime));
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

    protected override void Jump()
    {
        throw new System.NotImplementedException();
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
        
        Debug.DrawRay(position, -up.normalized * 1.2f, Color.green);
        
        return Physics.Raycast(position, up, playerHeight * 1.2f, groundLayer.value);
    }

    protected Vector3 GetDirection()
    {
        Vector3 _out = Vector3.zero;

        _out += transform.forward * mMove.y;
        _out += transform.right * mMove.x;

        return _out.normalized;
    }
}
