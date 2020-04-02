using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(SkeletonRenderer))]
public class BloodController : MonoBehaviour
{
    public Action OnDeadHandle;
    
    const string ANIM_NAME = "Fill";
    SkeletonRenderer mSkeletonRenderer;
    Skeleton mSkeleton;
    Spine.Animation mAnimation;

    float mBlood;
    float mSpeed = 0.1f;
    
    void Start()
    {
        mSkeletonRenderer = GetComponent<SkeletonRenderer>();
        mSkeleton = mSkeletonRenderer.skeleton;
        mAnimation = mSkeleton.Data.FindAnimation(ANIM_NAME);
        mBlood = 1;
        SetPercent(mBlood);
    }

    public void Decrease()
    {
        mBlood -= mSpeed * Time.deltaTime;
        SetPercent(mBlood);
    }
    
    void SetPercent (float percent)
    {
        mBlood = Mathf.Clamp01(percent);
        mAnimation.Apply(mSkeleton, 0, mBlood, false, null, 1f, MixBlend.Setup, MixDirection.In);
        mSkeleton.Update(Time.deltaTime);
        mSkeleton.UpdateWorldTransform();
        if (mBlood == 0)
            OnDeadHandle?.Invoke();
    }
}
