using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity.AttachmentTools;
using UnityEngine;

public class CharacterSkinController : ICharacterController
{
    public string[] DefaultSkinArray;
    
    CharacterBaseController mBaseController;
    Skin mCurrentSkin = new Skin("temple");
    Dictionary<string, string> mSkinDic = new Dictionary<string, string>();
    Dictionary<EquipData, Attachment> mCachedAttachments = new Dictionary<EquipData, Attachment>();
    
    public CharacterSkinController(CharacterBaseController controller)
    {
        mBaseController = controller;
    }
    
    public void Start()
    {
        ShowDefaultSkin();
    }
    
    void ShowDefaultSkin()
    {
        if (DefaultSkinArray == null) return;
        mCurrentSkin.Clear();
        mSkinDic.Clear();
        foreach (var skin in DefaultSkinArray)
        {
            mCurrentSkin.AddAttachments(mBaseController.SkeletonData.FindSkin(skin));
            string[] value = skin.Split('/');
            if (value.Length == 2)
                mSkinDic[value[0]] = "/" + value[1];
            else
                mSkinDic[value[0]] = string.Empty;
        }

        mBaseController.Skeleton.SetSkin(mCurrentSkin);
        RefreshSkeletonAttachments();
    }

    public void UpdateSkin(string skinName)
    {
        if (mSkinDic.Count == 1)
        {
            mCurrentSkin.Clear();
            mCurrentSkin.AddAttachments(mBaseController.SkeletonData.FindSkin(skinName));
        }
        else
        {
            string[] skinValue = skinName.Split('/');
            if (skinValue.Length == 2)
            {
                mSkinDic[skinValue[0]] = "/" + skinValue[1];
                mCurrentSkin.Clear();
                foreach (var skin in mSkinDic)
                    mCurrentSkin.AddAttachments(mBaseController.SkeletonData.FindSkin(skin.Key + skin.Value));
            }
        }
        mBaseController.Skeleton.SetSkin(mCurrentSkin);
        RefreshSkeletonAttachments();
    }

    public void Equip(EquipData data, EquipConfig config)
    {
        int slotIndex = mBaseController.SkeletonData.FindSlotIndex(config.Slot);
        var attachment = GenerateAttachmentFromEquipAsset(data, slotIndex, config.Skin, config.Attachment);
        Equip(slotIndex, config.Attachment, attachment);
    }

    Attachment GenerateAttachmentFromEquipAsset (EquipData data, int slotIndex, string skinName, string attachmentName) {
        mCachedAttachments.TryGetValue(data, out Attachment attachment);
    
        if (attachment == null) {
            var templateSkin = mBaseController.SkeletonData.FindSkin(skinName);
            Attachment templateAttachment = templateSkin.GetAttachment(slotIndex, attachmentName);
            attachment = templateAttachment.GetRemappedClone(data.Image, mBaseController.SkeletonAnim.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial, premultiplyAlpha: true);
            mCachedAttachments.Add(data, attachment); // Cache this value for next time this asset is used.
        }
    
        return attachment;
    }
    
    void Equip (int slotIndex, string attachmentName, Attachment attachment) {
        mCurrentSkin.SetAttachment(slotIndex, attachmentName, attachment);
        mBaseController.Skeleton.SetSkin(mCurrentSkin);
        RefreshSkeletonAttachments();
    }
    
    void RefreshSkeletonAttachments () {
        mBaseController.Skeleton.SetSlotsToSetupPose();
        mBaseController.SkeletonAnim.state.Apply(mBaseController.Skeleton); //skeletonAnimation.Update(0);
    }

    public void Update(float deltaTime)
    {
    }

    public void Destroy()
    {
    }
}
