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
    
    public GameObject EnemyPrefab;
    public Transform[] EnemySpawnPoint;

    void Start()
    {
        CharacterManager.Instance.Start();
        InputManager.Instance.Start();
        
        GameObject character = CharacterManager.Instance.CreateCharacter(CharacterPrefab, SpawnPoint.position, BloodPrefab, true);
        CameraFollow.Target = character.transform;
        
        foreach (var point in EnemySpawnPoint)
            CharacterManager.Instance.CreateCharacter(EnemyPrefab, point.position, BloodPrefab, false);
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        CharacterManager.Instance.Update(deltaTime);
        InputManager.Instance.Update(deltaTime);
    }

    void OnDestroy()
    {
        CharacterManager.Instance.Destroy();
        InputManager.Instance.Destroy();
    }
}
