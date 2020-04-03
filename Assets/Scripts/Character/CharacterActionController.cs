using Spine;
using UnityEngine;

public enum ECharacterActionState
{
    None,
    Idle,
    Walk,
    Run,
    Jump,
    Dead,
}

public class CharacterActionController : ICharacterController
{
    public float WalkSpeed = 5;
    public float RunSpeed = 10;
    public float JumpSpeed = 5;
    public EForward DefaultForward;

    CharacterBaseController mBaseController;
    CharacterAnimationController mAnimationController;
    Transform mTransform;

    ECharacterActionState mCurrentActionState;
    public ECharacterActionState CurrentActionState => mCurrentActionState;
    
    Vector3 mVelocity = default(Vector3);
    bool mIsShoot = false;
    float mShootTime = 0;
    float ShootInterval = 0.2f;

    public CharacterActionController(CharacterBaseController controller)
    {
        mBaseController = controller;
        mAnimationController = mBaseController.AnimationController;
        mTransform = mBaseController.Transform;
    }

    public void Start()
    {
        SetNextState(ECharacterActionState.Idle);
    }

    public void Update()
    {
        if (mCurrentActionState == ECharacterActionState.Dead) return;
        float timeDelta = Time.deltaTime;
        if (mBaseController.CharacterController == null)
        {
            var move = Quaternion.Euler(0, mTransform.eulerAngles.y, 0) * new Vector3(mVelocity.x,0);
            mTransform.position += move * timeDelta;
        }
        else
        {
            bool isGrounded = mBaseController.CharacterController.isGrounded;
            Vector3 gravityDeltaVelocity = Physics.gravity * timeDelta;
            if (!isGrounded)
                mVelocity += gravityDeltaVelocity;
            
            mBaseController.CharacterController.Move(mVelocity * timeDelta);
        }
        
        if (mIsShoot)
            Shoot(timeDelta);
    }

    public void UpdateForward(float x)
    {
        if(x.Equals(0)) return;
        if ((x < 0 && DefaultForward == EForward.Left) || (x > 0 && DefaultForward == EForward.Right))
            mBaseController.Skeleton.ScaleX = 1;
        else
            mBaseController.Skeleton.ScaleX = -1;
    }

    void SetNextState(ECharacterActionState state)
    {
        if (state == mCurrentActionState) return;
        if (mCurrentActionState == ECharacterActionState.Dead) return;
        if (mCurrentActionState == ECharacterActionState.Jump) return;
        mCurrentActionState = state;
        UpdateAnimationWithState();
    }

    void UpdateAnimationWithState()
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
                mAnimationController.SetOnceAnimation(mAnimationController.JumpAnim, JumpEnd);
                break;
            case ECharacterActionState.Dead:
                mAnimationController.SetOnceAnimation(mAnimationController.DeadAnim);
                break;
        }
    }

    public void SetMoveSpeed(float value, bool isRun)
    {
        if (value.Equals(0))
        {
            SetNextState(ECharacterActionState.Idle);
            mVelocity.x = 0;
            return;
        }
        if (isRun)
        {
            mVelocity.x = value * RunSpeed;
            SetNextState(ECharacterActionState.Run);
        }
        else
        {
            mVelocity.x = value * WalkSpeed;
            SetNextState(ECharacterActionState.Walk);
        }
    }

    public void Jump()
    {
        mVelocity.y = JumpSpeed;
        SetNextState(ECharacterActionState.Jump);
    }

    void JumpEnd(TrackEntry trackEntry)
    {
        mVelocity.y = 0;
        mCurrentActionState = ECharacterActionState.Idle;
        UpdateAnimationWithState();
    }

    public void ShootStart()
    {
        mBaseController.BoneController.ResetShootBone();
        mShootTime = ShootInterval;
        mIsShoot = true;
    }
    
    public void ShootCancel()
    {
        mIsShoot = false;
    }

    void Shoot(float time)
    {
        mShootTime += time;
        if (mShootTime >= ShootInterval)
        {
            mAnimationController.SetOnceCombiningAnimation(mAnimationController.ShootAnim);
            mAnimationController.SetAimAnimation();
            mShootTime = 0;
        }
    }

    public void UpdateShootBone(Vector2 value)
    {
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

    public void Skill1()
    {
        mAnimationController.SetOnceCombiningAnimation(mAnimationController.Skill1Anim);
    }

    public void Skill2()
    {
        mAnimationController.SetOnceCombiningAnimation(mAnimationController.Skill2Anim);
    }

    public void Skill3()
    {
        mAnimationController.SetOnceCombiningAnimation(mAnimationController.Skill3Anim);
    }

    public void Dead()
    {
        SetNextState(ECharacterActionState.Dead);
    }
    
    public void Destroy()
    {
        mCurrentActionState = ECharacterActionState.None;
        mIsShoot = false;
    }
}