using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class CharacterManager : BaseManager<CharacterManager>
{
    CharacterBaseController mMine;
    public CharacterBaseController Mine => mMine;

    public override void Start()
    {
        base.Start();
    }

    public GameObject CreateCharacter(GameObject prefab, Vector3 position, GameObject blood, bool isMine)
    {
        GameObject go = Object.Instantiate(prefab, position, Quaternion.identity);
        CharacterBaseController mCharacterBaseController = go.GetComponentInChildren<SkeletonAnimation>().gameObject
            .AddComponent<CharacterBaseController>();
        mCharacterBaseController.InitDataByConfig(go.GetComponent<CharacterConfig>());
        mCharacterBaseController.CreateBloodObject(blood, go.transform);
        if (isMine)
            mMine = mCharacterBaseController;
        else
            mCharacterBaseController.AddAIController();
        return go;
    }

    public void TestToSetMine(CharacterBaseController character)
    {
        mMine = character;
    }
}
