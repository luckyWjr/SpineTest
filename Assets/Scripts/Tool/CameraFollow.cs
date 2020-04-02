using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FollowDirection
{
    Vertical,
    Horizontal,
    All
}

public class CameraFollow : MonoBehaviour
{
    public Transform Target;

    public FollowDirection Direction = FollowDirection.All;
    Transform mTransform;
    Vector3 mOffset = new Vector3(0, 4, -20);

    void Start()
    {
        mTransform = transform;
        mOffset = mTransform.position;
    }

    void LateUpdate()
    {
        if (Target == null) return;
        switch (Direction)
        {
            case FollowDirection.All:
                mTransform.position = Target.position + mOffset;
                break;
            case FollowDirection.Horizontal:
                mTransform.position = mOffset + new Vector3(Target.position.x, 0, 0);
                break;
            case FollowDirection.Vertical:
                mTransform.position = mOffset + new Vector3(0, Target.position.y, 0);
                break;
        }
    }
}
