using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(SkeletonAnimation))]
public class CharacterBaseController : MonoBehaviour
{
    CharacterController mCharacterController;

    public CharacterController CharacterController
    {
        get
        {
            if (mCharacterController == null)
                mCharacterController = mTransform.parent.GetComponent<CharacterController>();
            return mCharacterController;
        }
    }
    
    SkeletonAnimation mSkeletonAnim;
    public SkeletonAnimation SkeletonAnim
    {
        get
        {
            if (mSkeletonAnim == null)
                mSkeletonAnim = GetComponentInChildren<SkeletonAnimation>();
            return mSkeletonAnim;
        }
    }
    
    Skeleton mSkeleton;//有状态的对象，可以设置骨架的姿势或者替换一个槽的附件
    public Skeleton Skeleton
    {
        get
        {
            if (mSkeleton == null && SkeletonAnim != null)
                mSkeleton = SkeletonAnim.skeleton;
            return mSkeleton;
        }
    }
    
    SkeletonData mSkeletonData;//无状态的对象，可以获得一些信息，例如Animation的持续性、Event、或者Setup/Bind姿势
    public SkeletonData SkeletonData
    {
        get
        {
            if (mSkeletonData == null && Skeleton != null)
                mSkeletonData = Skeleton.Data;
            return mSkeletonData;
        }
    }

    public Spine.AnimationState AnimationState { get; private set; }

    Transform mTransform;
    public Transform Transform
    {
        get
        {
            if (mTransform == null)
                mTransform = transform;
            return mTransform;
        }
    }

    public CharacterActionController ActionController;
    public CharacterAnimationController AnimationController;
    public CharacterSkinController SkinController;
    public CharacterBoneController BoneController;
    public CharacterAIController AIController;
    public BloodController BloodController;
    
    void Awake()
    {
        AnimationController = new CharacterAnimationController(this);
        SkinController = new CharacterSkinController(this);
        ActionController = new CharacterActionController(this);
        BoneController = new CharacterBoneController(this);
    }

    void Start()
    {
        //start之后才可获取
        AnimationState = SkeletonAnim.state;
        AnimationController.Start();
        ActionController.Start();
        SkinController.Start();
        BoneController.Start();
        AIController?.Start();
    }

    public void AddAIController()
    {
        AIController = new CharacterAIController(this);
    }

    public void CreateBloodObject(GameObject prefab, Transform parent)
    {
        BloodController = Object.Instantiate(prefab).GetComponent<BloodController>();
        BloodController.OnDeadHandle = Dead;
        Transform tran = BloodController.transform;
        tran.SetParent(parent);
        tran.localScale = Vector3.one;
        tran.localPosition = new Vector3(0, 8, 0);
    }
    
    void Update()
    {
        float deltaTime = Time.deltaTime;
        AnimationController.Update(deltaTime);
        ActionController.Update(deltaTime);
        AIController?.Update(deltaTime);

        if (CharacterController != null && ActionController.CurrentActionState != ECharacterActionState.Dead)
        {
            Ray ray = new Ray(mTransform.position + Vector3.up * 3, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 5.5f))
                if (hit.transform.name.Equals("Trap"))
                    BloodController.Decrease();
        }
    }

    void Dead()
    {
        ActionController.Dead();
        InputManager.Instance.Disable();
    }
    
    void Destroy()
    {
        ActionController.Destroy();
    }

    public void InitDataByConfig(SpineAssetConfig config)
    {
        SkinController.DefaultSkinArray = config.DefaultSkinArray;
        
        ActionController.DefaultForward = config.DefaultForward;
        ActionController.WalkSpeed = config.WalkSpeed;
        ActionController.RunSpeed = config.RunSpeed;

        AnimationController.IdleAnim = config.IdleAnim;
        AnimationController.WalkAnim = config.WalkAnim;
        AnimationController.RunAnim = config.RunAnim;
        AnimationController.JumpAnim = config.JumpAnim;
        AnimationController.ShootAnim = config.ShootAnim;
        AnimationController.Skill1Anim = config.Skill1Anim;
        AnimationController.Skill2Anim = config.Skill2Anim;
        AnimationController.Skill3Anim = config.Skill3Anim;
    }
    
    public void InitDataByConfig(CharacterConfig config)
    {
        SkinController.DefaultSkinArray = config.DefaultSkinArray;
        
        ActionController.DefaultForward = config.DefaultForward;
        ActionController.WalkSpeed = config.WalkSpeed;
        ActionController.RunSpeed = config.RunSpeed;
        ActionController.JumpSpeed = config.JumpSpeed;

        AnimationController.IdleAnim = config.IdleAnim;
        AnimationController.WalkAnim = config.WalkAnim;
        AnimationController.RunAnim = config.RunAnim;
        AnimationController.JumpAnim = config.JumpAnim;
        AnimationController.AimAnim = config.AimAnim;
        AnimationController.ShootAnim = config.ShootAnim;
        AnimationController.Skill1Anim = config.Skill1Anim;
        AnimationController.Skill2Anim = config.Skill2Anim;
        AnimationController.Skill3Anim = config.Skill3Anim;
        AnimationController.DeadAnim = config.DeadAnim;
        
        BoneController.ShootBoneName = config.ShootBone;
    }
}