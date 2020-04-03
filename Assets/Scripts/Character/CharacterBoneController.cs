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
        if (!string.IsNullOrEmpty(ShootBoneName))
        {
            mShootBone = mBaseController.Skeleton.FindBone(ShootBoneName);
            mShootBoneDefaultPosition = mShootBone.GetLocalPosition();
        }
    }

    public void Update(float deltaTime)
    {
    }
    
    public void Destroy()
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