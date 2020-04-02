using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class CharacterConfig : MonoBehaviour
{
    public EForward DefaultForward;
    [SpineSkin()] public string[] DefaultSkinArray;

    [Header("Moving Setting")]
    public float WalkSpeed;
    public float RunSpeed;
    public float JumpSpeed;
    
    [Header("Animation Setting")]
    [SpineAnimation()]public string IdleAnim;
    [SpineAnimation()]public string WalkAnim;
    [SpineAnimation()]public string RunAnim;
    [SpineAnimation()]public string JumpAnim;
    [SpineAnimation()]public string AimAnim;
    [SpineAnimation()]public string ShootAnim;
    [SpineAnimation()]public string Skill1Anim;
    [SpineAnimation()]public string Skill2Anim;
    [SpineAnimation()]public string Skill3Anim;
    [SpineAnimation()]public string DeadAnim;

    [Header("Bone Setting")]
    [SpineBone()]public string ShootBone;
}
