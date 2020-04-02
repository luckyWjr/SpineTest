using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class CharacterManager
{
    static  CharacterManager mInstance;
    public static CharacterManager Instance => mInstance ?? (mInstance = new CharacterManager());
    
    public GameObject CreateCharacter(GameObject prefab, Vector3 position, GameObject blood)
    {
        GameObject go = Object.Instantiate(prefab, position, Quaternion.identity);
        CharacterBaseController mCharacterBaseController = go.GetComponentInChildren<SkeletonAnimation>().gameObject.AddComponent<CharacterBaseController>();
        mCharacterBaseController.InitDataByConfig(go.GetComponent<CharacterConfig>());
        mCharacterBaseController.CreateBloodObject(blood, go.transform);
        return go;
    }
}
