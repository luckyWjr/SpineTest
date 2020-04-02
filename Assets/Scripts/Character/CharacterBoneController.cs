using Spine;
using Spine.Unity;
using UnityEngine;

public class CharacterBoneController : ICharacterController
{
    public string ShootBoneName;
    
    Bone mShootBone;
    Vector2 mShootBoneDefaultPosition;

    CharacterBaseController mBaseController;

    public CharacterBoneController(CharacterBaseController controller)
    {
        mBaseController = controller;
    }
    
    public void Start()
    {
        if (ShootBoneName != null)
        {
            mShootBone = mBaseController.Skeleton.FindBone(ShootBoneName);
            mShootBoneDefaultPosition = mShootBone.GetLocalPosition();
        }
    }

    public void Update()
    {
    }

    public void Enable()
    {
    }

    public void Disable()
    {
    }

    public void UpdateShootBone(Vector2 point)
    {
        mShootBone?.SetLocalPosition(point);
    }
    
    public void ResetShootBone()
    {
        mShootBone?.SetLocalPosition(mShootBoneDefaultPosition);
    }
}