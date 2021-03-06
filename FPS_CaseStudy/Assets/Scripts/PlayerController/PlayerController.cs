﻿using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.Input;

public abstract class PlayerController : MonoBehaviour
{
    [SerializeField]
    protected FPSControls test;

    [SerializeField, Required] 
    protected Camera camera;

    [SerializeField,ReadOnly,FoldoutGroup("Information")]
    protected Vector2 mMove;
    [SerializeField,ReadOnly,FoldoutGroup("Information")]
    protected Vector2 mLook;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        InitControls();

    }

    protected virtual void InitControls()
    {
        test.Gameplay.Move.Enable();
        test.Gameplay.Move.performed += ctx => { mMove = ctx.ReadValue<Vector2>(); };
        
        test.Gameplay.Look.Enable();
        test.Gameplay.Look.performed += ctx => { mLook = ctx.ReadValue<Vector2>(); };
        
        test.Gameplay.Jump.Enable();
        test.Gameplay.Jump.performed += ctx => Jump();
        
        test.Gameplay.Fire.Enable();
        test.Gameplay.Fire.performed += ctx => Fire();
    }

    protected abstract void ProcessMove();
    protected abstract void ProcessLook();

    protected abstract void Jump();

    protected abstract void Fire();

    protected abstract void Aim();

    protected abstract void Reload();

    protected abstract bool IsOnGround();
}
