using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using AnimationState = Spine.AnimationState;

public class CharacterAnimationController : ICharacterController
{
    CharacterBaseController mBaseController;
    const int mDefaultTrack = 0;
    const int mSkillTrack = 1;
    const int mAimTrack = 2;
    const float mEmptyAnimationDefaultMixDuring = 0.5f;
    const float mEmptyAnimationDefaultMixDelay = 0.1f;
    float mCurrentTimeScale = 1;
    Spine.AnimationState mAnimationState;
    TrackEntry mCurrentTrackEntry;
    TrackEntry mCurrentSkillTrackEntry;
    
    public string IdleAnim;
    public string WalkAnim;
    public string RunAnim;
    public string JumpAnim;
    public string AimAnim;
    public string ShootAnim;
    public string Skill1Anim;
    public string Skill2Anim;
    public string Skill3Anim;
    public string DeadAnim;
    
    public CharacterAnimationController(CharacterBaseController controller)
    {
        mBaseController = controller;
    }
   
    public void Start()
    {
        mAnimationState = mBaseController.AnimationState;
    }

    public void Update(float deltaTime)
    {
        // if (mCurrentSkillTrackEntry != null && mCurrentSkillTrackEntry.IsComplete)
        // {
        //     mAnimationState.SetEmptyAnimation(mSkillTrack, mEmptyAnimationDefaultMixDuring);
        //     mCurrentSkillTrackEntry = null;
        // }
    }
    
    public void SetTimeScale(float scale)
    {
        mCurrentTimeScale = scale;
        mAnimationState.TimeScale = mCurrentTimeScale;
    }

    public void StopOrStart()
    {
        if(mAnimationState.TimeScale > 0)
            mAnimationState.TimeScale = 0;
        else
            mAnimationState.TimeScale = mCurrentTimeScale;
    }
    
    public void SetLoopAnimation(string animName)
    {
        if(string.IsNullOrEmpty(animName)) return; 
        mCurrentTrackEntry = mAnimationState.SetAnimation(mDefaultTrack, animName, true);
    }

    public void SetOnceAnimation(string animName, AnimationState.TrackEntryDelegate completeCallback = null)
    {
        if (string.IsNullOrEmpty(animName))
        {
            completeCallback?.Invoke(null);
            return;
        }
        mCurrentTrackEntry = mAnimationState.SetAnimation(mDefaultTrack, animName, false);
        mCurrentTrackEntry.Complete += completeCallback;
    }

    public void SetOnceCombiningAnimation(string animName)
    {
        if(string.IsNullOrEmpty(animName)) return;
        mCurrentSkillTrackEntry = mAnimationState.SetAnimation(mSkillTrack, animName, false);
        mCurrentSkillTrackEntry.AttachmentThreshold = 1;
        AddEmptyAnimation(mSkillTrack);
    }

    public void SetAimAnimation()
    {
        var aimTrack = mAnimationState.SetAnimation(mAimTrack, AimAnim, false);
        aimTrack.AttachmentThreshold = 1f;
        aimTrack.MixDuration = 0f;
        AddEmptyAnimation(mAimTrack);
    }

    void AddEmptyAnimation(int track)
    {
        var empty = mAnimationState.AddEmptyAnimation(track, mEmptyAnimationDefaultMixDuring, mEmptyAnimationDefaultMixDelay);
        empty.AttachmentThreshold = 1f;
    }

    public void Destroy()
    {
    }
}
