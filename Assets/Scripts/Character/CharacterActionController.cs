using System.IO;
using Spine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public enum ECharacterActionState
{
    None,
    Idle,
    Walk,
    Run,
    Jump,
}

public class CharacterActionController : ICharacterController
{
    public float MoveSpeed = 5;
    public float RunSpeed = 10;
    public float JumpSpeed = 5;
    public float ShootInterval = 0.2f;
    public EForward DefaultForward;

    CharacterBaseController mBaseController;
    CharacterAnimationController mAnimationController;
    Transform mTransform;
    CharacterInputAction mActionController;

    ECharacterActionState mLastActionState;
    ECharacterActionState mCurrentActionState;
    
    Vector3 mVelocity = default(Vector3);
    bool mIsMove = false;
    bool mIsRun = false;
    bool mIsShoot = false;
    float mShootTime = 0;

    public CharacterActionController(CharacterBaseController controller)
    {
        mBaseController = controller;
        mAnimationController = mBaseController.AnimationController;
        mTransform = mBaseController.Transform;
        InitInputAction();

        mLastActionState = ECharacterActionState.None;
    }

    public void Start()
    {
        Idle();
    }

    void InitInputAction()
    {
        mActionController = new CharacterInputAction();

        mActionController.Player.Move.started += MoveStart;
        mActionController.Player.Move.performed += MovePerformed;
        mActionController.Player.Move.canceled += MoveEnd;
        mActionController.Player.Run.started += RunStart;
        mActionController.Player.Run.canceled += RunEnd;
        mActionController.Player.Jump.performed += Jump;
        mActionController.Player.Shoot.started += ShootStart;
        mActionController.Player.Shoot.canceled += ShootCancel;
        mActionController.Player.Skill1.performed += Skill1;
        mActionController.Player.Skill2.performed += Skill2;
        mActionController.Player.Skill3.performed += Skill3;
    }

    void UpdateAnimation()
    {
        if (mCurrentActionState != mLastActionState)
        {
            switch (mCurrentActionState)
            {
                case ECharacterActionState.Idle:
                    mAnimationController.SetLoopAnimation(mAnimationController.IdleAnim);
                    break;
                case ECharacterActionState.Walk:
                    mAnimationController.SetLoopAnimation(mAnimationController.WalkAnim);
                    break;
                case ECharacterActionState.Run:
                    mAnimationController.SetLoopAnimation(mAnimationController.RunAnim);
                    break;
                case ECharacterActionState.Jump:
                    mAnimationController.SetOnceAnimation(mAnimationController.JumpAnim, e => { mCurrentActionState = ECharacterActionState.Idle; });
                    break;
            }

            mLastActionState = mCurrentActionState;
        }
    }


    public void Update()
    {
        float timeDelta = Time.deltaTime;
        if (mBaseController.CharacterController == null)
        {
            if (mIsMove)
            {
                var scaledMoveSpeed = MoveSpeed;
                if (mIsRun)
                    scaledMoveSpeed = RunSpeed;
                Vector2 v = mActionController.Player.Move.ReadValue<Vector2>();
                var move = Quaternion.Euler(0, mTransform.eulerAngles.y, 0) * new Vector3(v.x, v.y);
                mTransform.position += move * (scaledMoveSpeed * timeDelta);
            }
        }
        else
        {
            bool isGrounded = mBaseController.CharacterController.isGrounded;
            Vector3 gravityDeltaVelocity = Physics.gravity * timeDelta;
            if (mCurrentActionState == ECharacterActionState.Jump)
                mVelocity.y = JumpSpeed;
            if (!isGrounded)
                mVelocity += gravityDeltaVelocity;

            mVelocity.x = 0;
            if (mIsMove)
                mVelocity.x = mActionController.Player.Move.ReadValue<Vector2>().x * (mIsRun ? RunSpeed : MoveSpeed);

            mBaseController.CharacterController.Move(mVelocity * timeDelta);
        }
        
        if (mIsShoot)
            Shoot(mActionController.Player.Shoot.ReadValue<Vector2>(), timeDelta);

        if (mIsMove && mCurrentActionState != ECharacterActionState.Jump)
            mCurrentActionState = mIsRun ? ECharacterActionState.Run : ECharacterActionState.Walk;
        UpdateAnimation();
    }

    void Idle()
    {
        if (mCurrentActionState == ECharacterActionState.Jump) return;
        mCurrentActionState = ECharacterActionState.Idle;
    }

    void MoveStart(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().x == 0) return;
        mIsMove = true;
    }

    void MovePerformed(InputAction.CallbackContext context)
    {
        Vector2 detail = context.ReadValue<Vector2>();
        if (detail.x < 0)
        {
            if (DefaultForward == EForward.Left)
                mBaseController.Skeleton.ScaleX = 1;
            else
                mBaseController.Skeleton.ScaleX = -1;
        }
        else if (detail.x > 0)
        {
            if (DefaultForward == EForward.Left)
                mBaseController.Skeleton.ScaleX = -1;
            else
                mBaseController.Skeleton.ScaleX = 1;
        }
    }

    void MoveEnd(InputAction.CallbackContext context)
    {
        mIsMove = false;
        Idle();
    }

    void RunStart(InputAction.CallbackContext context)
    {
        mIsRun = true;
    }

    void RunEnd(InputAction.CallbackContext context)
    {
        mIsRun = false;
    }
    
    void Jump(InputAction.CallbackContext context)
    {
        mCurrentActionState = ECharacterActionState.Jump;
    }

    void ShootStart(InputAction.CallbackContext context)
    {
        mBaseController.BoneController.ResetShootBone();
        mShootTime = ShootInterval;
        mIsShoot = true;
    }
    
    void ShootCancel(InputAction.CallbackContext context)
    {
        mIsShoot = false;
    }

    void Shoot(Vector2 value, float time)
    {
        mShootTime += time;
        if (mShootTime >= ShootInterval)
        {
            mAnimationController.SetOnceCombiningAnimation(mAnimationController.ShootAnim);
            mAnimationController.SetAimAnimation();
            mShootTime = 0;
            
            if (Mathf.Abs(value.x) >= 0.1f || Mathf.Abs(value.y) >= 0.1f)
            {
                var forwardPosition = mBaseController.SkeletonAnim.transform.position +
                                         new Vector3(value.x * 50, value.y * 50, 0);
                var shootPoint = mBaseController.SkeletonAnim.transform.InverseTransformPoint(forwardPosition);
                shootPoint.x *= mBaseController.Skeleton.ScaleX;
                shootPoint.y *= mBaseController.Skeleton.ScaleY;
                mBaseController.BoneController.UpdateShootBone(shootPoint);
            }
        }
    }

    void Skill1(InputAction.CallbackContext context)
    {
        mAnimationController.SetOnceCombiningAnimation(mAnimationController.Skill1Anim);
    }

    void Skill2(InputAction.CallbackContext context)
    {
        mAnimationController.SetOnceCombiningAnimation(mAnimationController.Skill2Anim);
    }

    void Skill3(InputAction.CallbackContext context)
    {
        mAnimationController.SetOnceCombiningAnimation(mAnimationController.Skill3Anim);
    }
    
    public void Enable()
    {
        mActionController.Enable();
    }

    public void Disable()
    {
        mCurrentActionState = ECharacterActionState.None;
        mIsMove = false;
        mIsRun = false;
        mIsShoot = false;
        mActionController.Disable();
    }
}