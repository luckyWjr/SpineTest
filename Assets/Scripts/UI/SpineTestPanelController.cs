using System;
using System.Collections.Generic;
using System.IO;
using Spine;
using UnityEngine;
using UnityEngine.UI;

public class SpineTestPanelController : MonoBehaviour
{
    const string SPRITE_FOLDER_PATH = "Equips/";
    const string ASSET_FOLDER_PATH = "EquipAssets/";
    const string DEMO_RESOURCES_FOLDER_PATH = "/Resources/";
    
    public Dropdown AnimDropdown;
    public Dropdown SkinDropdown;
    public InputField TimeScaleInputField;
    public Button PlayButton;
    public Text LogText;
    public Transform SpritesRoot;
    public Transform SpriteButtonPrefab;
    public SpineAssetConfig Config;

    CharacterBaseController mCharacterBaseController;
    List<Dropdown.OptionData> mAnimNameList = new List<Dropdown.OptionData>();
    List<Dropdown.OptionData> mSkinNameList = new List<Dropdown.OptionData>();
    List<EquipData> mEquipDataList = new List<EquipData>();

    void Awake()
    {
        mCharacterBaseController = Config.SpineRoot.GetComponentInChildren<CharacterBaseController>();
        if (mCharacterBaseController != null)
        {
            SetUIEvent();
            if (Config.ReadMethod == EReadEquipMethod.ReadSprite)
                ReadAllSpriteInFolder();
            else
                ReadAllAssetInFolder();
            
            DisplaySprites();
        }
        else
            ShowHintText("在SpineRoot节点下没有找到动画文件", false);
    }

    void Start()
    {
        if (mCharacterBaseController != null)
        {
            mCharacterBaseController.InitDataByConfig(Config);
            UpdateAnimData();
            UpdateSkinData();
        }
    }

    void SetUIEvent()
    {
        AnimDropdown.onValueChanged.AddListener(ChangeAnim);
        SkinDropdown.onValueChanged.AddListener(ChangeSkin);
        TimeScaleInputField.onEndEdit.AddListener(ChangeTimeScale);
        PlayButton.onClick.AddListener(OnPlayClick);
    }

    void UpdateAnimData()
    {
        mAnimNameList.Clear();
        foreach (var anim in mCharacterBaseController.SkeletonData.Animations)
            mAnimNameList.Add(new Dropdown.OptionData(anim.Name));
        AnimDropdown.options = mAnimNameList;
    }

    void UpdateSkinData()
    {
        mSkinNameList.Clear();
        int defaultSkinIndex = 0;
        Skin[] array = mCharacterBaseController.SkeletonData.Skins.Items;
        for (int i = 0; i < array.Length; i++)
        {
            mSkinNameList.Add(new Dropdown.OptionData(array[i].Name));
            if (Config.DefaultSkinArray != null && array[i].Name == Config.DefaultSkinArray[0])
                defaultSkinIndex = i;
        }
        SkinDropdown.options = mSkinNameList;
        SkinDropdown.value = defaultSkinIndex;
    }

    void ChangeAnim(int value)
    {
        if (value < mAnimNameList.Count)
            mCharacterBaseController.AnimationController.SetLoopAnimation(mAnimNameList[value].text);
    }

    void ChangeSkin(int value)
    {
        if (value < mSkinNameList.Count)
        {
            mCharacterBaseController.SkinController.UpdateSkin(mSkinNameList[value].text);
        }
    }

    void ChangeTimeScale(string value)
    {
        if (float.TryParse(value, out float scale))
        {
            scale = Mathf.Clamp(scale,0, 100);
            mCharacterBaseController.AnimationController.SetTimeScale(scale);
        }
    }
    
    void OnPlayClick()
    {
        mCharacterBaseController.AnimationController.StopOrStart();
    }
    
    void ShowHintText(string text, bool isTrue = true)
    {
        Debug.Log(text);
        LogText.text = text;
        if(isTrue)
            LogText.color = Color.green;
        else
            LogText.color = Color.red;
    }

    void ReadAllSpriteInFolder()
    {
        mEquipDataList.Clear();
        DirectoryInfo equipsDir = new DirectoryInfo(Application.dataPath + DEMO_RESOURCES_FOLDER_PATH + SPRITE_FOLDER_PATH);
        DirectoryInfo[] typeDirArray = equipsDir.GetDirectories();
        foreach (var dir in typeDirArray)
        {
            string dirName = dir.Name;
            FileInfo[] spriteFileArray = dir.GetFiles("*.png", SearchOption.AllDirectories);
            foreach (var spriteFile in spriteFileArray)
            {
                Sprite sprite = Resources.Load<Sprite>(SPRITE_FOLDER_PATH + dirName + "/" + spriteFile.Name.Split('.')[0]);
                EquipData data = ScriptableObject.CreateInstance<EquipData>();
                data.Type = dirName;
                data.Image = sprite;
                mEquipDataList.Add(data);
            }
        }
        if(mEquipDataList.Count == 0)
            ShowHintText($"{DEMO_RESOURCES_FOLDER_PATH + SPRITE_FOLDER_PATH}目录下没有读取到换装的图片资源");
    }

    void ReadAllAssetInFolder()
    {
        mEquipDataList.Clear();
        DirectoryInfo equipsDir = new DirectoryInfo(Application.dataPath + DEMO_RESOURCES_FOLDER_PATH + ASSET_FOLDER_PATH);
        DirectoryInfo[] typeDirArray = equipsDir.GetDirectories();
        foreach (var dir in typeDirArray)
        {
            string dirName = dir.Name;
            FileInfo[] spriteFileArray = dir.GetFiles("*.asset", SearchOption.AllDirectories);
            foreach (var spriteFile in spriteFileArray)
            {
                EquipData data = Resources.Load<EquipData>(ASSET_FOLDER_PATH + dirName + "/" + spriteFile.Name.Split('.')[0]);
                data.Type = dirName;
                mEquipDataList.Add(data);
            }
        }
        if(mEquipDataList.Count == 0)
            ShowHintText($"{DEMO_RESOURCES_FOLDER_PATH + ASSET_FOLDER_PATH}目录下没有读取到换装的Asset资源");
    }

    void DisplaySprites()
    {
        foreach (var data in mEquipDataList)
        {
            SpriteButtonController image = Instantiate(SpriteButtonPrefab).GetComponent<SpriteButtonController>();
            image.transform.SetParent(SpritesRoot);
            image.Data = data;
            image.Panel = this;
        }
    }
    
    public void Equip (EquipData data) {
        EquipConfig config = Config.EquipConfigList.Find(x => x.Type == data.Type);
        if (config != null)
            mCharacterBaseController.SkinController.Equip(data, config);
    }
}
