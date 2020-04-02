using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameLaunch : MonoBehaviour
{
    public CameraFollow CameraFollow;
    public GameObject BloodPrefab;
    public GameObject CharacterPrefab;
    public Transform SpawnPoint;

    void Start()
    {
        GameObject character = CharacterManager.Instance.CreateCharacter(CharacterPrefab, SpawnPoint.position, BloodPrefab);
        CameraFollow.Target = character.transform;
    }
}
