using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[Serializable]
public class EquipConfig
{
    public string Type;
    [SpineSlot] public string Slot;
    [SpineSkin] public string Skin;
    [SpineAttachment(skinField: "Skin")] public string Attachment;
}

public enum EReadEquipMethod
{
    ReadSprite,
    ReadAsset,
}

public enum EForward
{
    Left,
    Right,
}

public class SpineAssetConfig : MonoBehaviour
{
    [Header("Default Setting")]
    public SkeletonDataAsset Asset;
    public Transform SpineRoot;
    public EReadEquipMethod ReadMethod;
    public List<EquipConfig> EquipConfigList = new List<EquipConfig>();
    [SpineSkin()] public string[] DefaultSkinArray;
    
    //动作相关设置
    public float MoveSpeed = 5;
    public float RunSpeed = 10;
    public EForward DefaultForward;
    
    [Header("Animation Setting")]
    [SpineAnimation()]public string IdleAnim;
    [SpineAnimation()]public string WalkAnim;
    [SpineAnimation()]public string RunAnim;
    [SpineAnimation()]public string JumpAnim;
    [SpineAnimation()]public string ShootAnim;
    [SpineAnimation()]public string Skill1Anim;
    [SpineAnimation()]public string Skill2Anim;
    [SpineAnimation()]public string Skill3Anim;
}
