using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAIController : ICharacterController
{
    CharacterBaseController mBaseController;
    public CharacterAIController(CharacterBaseController controller)
    {
        mBaseController = controller;
    }

    float mMoveTime;
    int mMoveForward;
    float mMovePeriod = 2;
    
    public void Start()
    {
        mMoveTime = mMovePeriod * 0.5f;
        mMoveForward = 1;
    }

    public void Update(float deltaTime)
    {
        mMoveTime += deltaTime;
        if (mMoveTime > mMovePeriod)
        {
            mMoveTime = 0;
            mMoveForward = -mMoveForward;
            mBaseController.ActionController.UpdateForward(mMoveForward);
        }
        mBaseController.ActionController.SetMoveSpeed(mMoveForward, false);
    }

    public void Destroy()
    {
    }
}
