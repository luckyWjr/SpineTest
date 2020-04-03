using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : BaseManager<InputManager>
{
    CharacterInputAction mInputAction;
    CharacterActionController mTemplateActionController;

    bool mIsWalkPerformed;
    bool mIsRunPerformed;

    CharacterActionController mActionController =>
        mTemplateActionController ??
        (mTemplateActionController = CharacterManager.Instance.Mine.ActionController);

    public override void Start()
    {
        base.Start();
        InitInputAction();
        ResetData();
    }

    void ResetData()
    {
        mIsWalkPerformed = false;
        mIsRunPerformed = false;
    }

    void InitInputAction()
    {
        mInputAction = new CharacterInputAction();

        mInputAction.Player.Walk.started += WalkStarted;
        mInputAction.Player.Walk.performed += WalkPerformed;
        mInputAction.Player.Walk.canceled += WalkEnd;
        mInputAction.Player.Run.performed += RunPerformed;
        mInputAction.Player.Run.canceled += RunEnd;
        mInputAction.Player.Jump.performed += Jump;
        mInputAction.Player.Shoot.started += ShootStarted;
        mInputAction.Player.Shoot.performed += ShootPerformed;
        mInputAction.Player.Shoot.canceled += ShootCancel;
        mInputAction.Player.Skill1.performed += Skill1;
        mInputAction.Player.Skill2.performed += Skill2;
        mInputAction.Player.Skill3.performed += Skill3;
        
        mInputAction.Enable();
    }
    
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (mIsWalkPerformed)
        {
            Vector2 detail = mInputAction.Player.Walk.ReadValue<Vector2>();
            mActionController.SetMoveSpeed(detail.x, mIsRunPerformed);
        }
    }

    void WalkStarted(InputAction.CallbackContext context)
    {
        mIsWalkPerformed = true;
    }

    void WalkPerformed(InputAction.CallbackContext context)
    {
        mActionController.UpdateForward(context.ReadValue<Vector2>().x);
    }
    
    void WalkEnd(InputAction.CallbackContext context)
    {
        mIsWalkPerformed = false;
        mActionController.SetMoveSpeed(0, false);
    }
    
    void RunPerformed(InputAction.CallbackContext context)
    {
        mIsRunPerformed = true;
    }
    
    void RunEnd(InputAction.CallbackContext context)
    {
        mIsRunPerformed = false;
    }
    
    void Jump(InputAction.CallbackContext context)
    {
        mActionController.Jump();
    }
    
    void ShootStarted(InputAction.CallbackContext context)
    {
        mActionController.ShootStart();
    }
    
    void ShootPerformed(InputAction.CallbackContext context)
    {
        mActionController.UpdateShootBone(context.ReadValue<Vector2>());
    }
    
    void ShootCancel(InputAction.CallbackContext context)
    {
        mActionController.ShootCancel();
    }
    
    void Skill1(InputAction.CallbackContext context)
    {
        mActionController.Skill1();
    }
    
    void Skill2(InputAction.CallbackContext context)
    {
        mActionController.Skill2();
    }
    
    void Skill3(InputAction.CallbackContext context)
    {
        mActionController.Skill3();
    }

    public override void Destroy()
    {
        base.Destroy();
        Disable();
    }

    public void Disable()
    {
        mInputAction.Disable();
    }
}