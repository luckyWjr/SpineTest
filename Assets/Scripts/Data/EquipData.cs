using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equip Asset", menuName = "Spine/Equip Asset")]
public class EquipData : ScriptableObject
{
    public string Type;
    public Sprite Image;
}
