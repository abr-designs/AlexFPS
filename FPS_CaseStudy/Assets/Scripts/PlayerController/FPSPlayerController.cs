using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public class FPSPlayerController : PlayerController
{

    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    [SerializeField, FoldoutGroup("Checking Ground")]
    private LayerMask groundLayer;

    [SerializeField, FoldoutGroup("Checking Ground")]
    private float playerHeight;

    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////// 

    [SerializeField, ReadOnly, FoldoutGroup("Information")]
    private bool canMove = true;

    [SerializeField, ReadOnly, FoldoutGroup("Information")]
    private bool isRunning = false;
    [SerializeField, ReadOnly, FoldoutGroup("Information")]
    private bool isCrouching = false;

    [SerializeField, ReadOnly, FoldoutGroup("Information")]
    private bool onGround = true;

    [SerializeField, ReadOnly, FoldoutGroup("Information")]
    private bool ignoreGround = false;

    [SerializeField, ReadOnly, FoldoutGroup("Information")]
    private bool isJumping = false;

    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    [SerializeField, ReadOnly, FoldoutGroup("Movement")]
    private Vector3 moveDelta;
    [SerializeField, ReadOnly, FoldoutGroup("Movement")]
    private Vector3 fallDelta;
    
    [SerializeField, FoldoutGroup("Movement")]
    private float moveSpeed = 5f;

    [SerializeField, FoldoutGroup("Movement")]
    private float runSpeed = 7f;
    
    [SerializeField, FoldoutGroup("Movement")]
    private float crouchSpeed = 2.5f;
    [FormerlySerializedAs("crouchHeight")] [SerializeField, FoldoutGroup("Movement")]
    private float crouchHeightReduction = 0.6f;

    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    [SerializeField, FoldoutGroup("Looking"), Required]
    private Transform playerCamera;

    [SerializeField, FoldoutGroup("Looking")]
    private float lookMultiplier = 1f;

    [SerializeField, FoldoutGroup("Looking")]
    private float maxLookAngle = 90f;

    private Vector3 startCameraLocation;
    private Quaternion startCameraRotation;

    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    [SerializeField, FoldoutGroup("Jumping")]
    private float jumpForce = 90f;


    private float jumpDelta;

    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    private Equipment equipment;
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
        equipment = GetComponent<Equipment>();

        //Init default values
        startCameraRotation = playerCamera.localRotation;

        isRunning = false;
        ignoreGround = false;
        canMove = true;
    }

    protected virtual void LateUpdate()
    {
        onGround = IsOnGround();
    }

    private void FixedUpdate()
    {
        //FIXME Think that i would prefer to have this in the Normal Update Loop
        ProcessLook();
        ProcessFalling();
        ProcessMove();

        rigidbody.position += (moveDelta + fallDelta) * Time.fixedDeltaTime;

    }

    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////

    protected override void InitControls()
    {
        base.InitControls();

        test.Gameplay.Sprint.Enable();
        test.Gameplay.Sprint.performed += ctx => { isRunning = ctx.ReadValue<float>() == 1f; };
        
        test.Gameplay.Crouch.Enable();
        test.Gameplay.Crouch.performed += ctx => { Crouch(ctx.ReadValue<float>()); };
        
        #if UNITY_EDITOR
        
        test.Debugging.Time.Enable();
        test.Debugging.Time.performed += ctx => { AdjustTimeScale(ctx.ReadValue<float>()); };
        
        test.Debugging.Pause.Enable();
        test.Debugging.Pause.performed += ctx =>
        {
            UnityEditor.EditorApplication.isPaused = ctx.ReadValue<float>() == 1f;
            
        };
        
        #endif
    }

    #region Controls

    protected override void ProcessLook()
    {
        if (mLook == Vector2.zero)
            return;

        //Used for rotating the player on the transform Y Axis
        Quaternion yRotation = rigidbody.rotation * Quaternion.Euler(0f, lookMultiplier * mLook.x, 0f);
        rigidbody.rotation = yRotation;


        //Used for Rotating the player camera on its local X Axis
        Quaternion xRotation = playerCamera.localRotation * Quaternion.Euler(lookMultiplier * mLook.y * -1f, 0f, 0f);

        if (Quaternion.Angle(startCameraRotation, xRotation) < maxLookAngle)
            playerCamera.localRotation = xRotation;

    }

    void ProcessFalling()
    {
        //var position = rigidbody.position;

        fallDelta = Vector3.up * jumpDelta;

        //Assuming that Gravity is always forcing down
        //jumpDelta += Physics.gravity.y * Time.fixedDeltaTime;

        if (ignoreGround)
            return;

        if (IsOnGround())
        {
            isJumping = false;
            canMove = true;
            jumpDelta = 0f;

            //Here i want to ensure that the rigidbody stops jumping around when it hits the ground
            //var vel = rigidbody.velocity;
            //vel.y = 0f;
            //rigidbody.velocity = vel;
        }
    }

    protected override void ProcessMove()
    {
        if (canMove == false)
            return;

        if (mMove == Vector2.zero)
        {
            moveDelta = Vector3.zero;
            return;
        }

        //I Want the crouching speed to override the running/walking speeds
        if(isCrouching)
            moveDelta = GetDirection() * crouchSpeed;
        else
        {
            moveDelta = GetDirection() * (isRunning ? runSpeed : moveSpeed);
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
       equipment.Fire(playerCamera.forward);
    }

    protected override void Reload()
    {
        throw new System.NotImplementedException();
    }

    protected virtual void Crouch(float value)
    {
        isCrouching = value == 1;
        
        playerCamera.localPosition = isCrouching ? startCameraLocation - (Vector3.up * crouchHeightReduction) : startCameraLocation;
    }

    #endregion //Controls

    protected override bool IsOnGround()
    {
        var position = transform.position;
        var up = transform.up;

        Debug.DrawRay(position, -up.normalized * playerHeight, Color.green);

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
    
    #region Debugging
    #if UNITY_EDITOR

    private void AdjustTimeScale(float amount)
    {
        if (amount < 0f)
            Time.timeScale /= 2f;
        else
            Time.timeScale *= 2f;
        
        Debug.Log("Set TimeScale to: " + Time.timeScale);
    }
    
    #endif
    #endregion //Debugging
    
}
