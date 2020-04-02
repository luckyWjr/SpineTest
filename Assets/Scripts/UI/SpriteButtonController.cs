using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteButtonController : MonoBehaviour
{
    public EquipData Data;
    public SpineTestPanelController Panel;
    Button mButton;
    Image mImage;
    Transform mTransform;
    
    void Start()
    {
        mTransform = transform;
        mTransform.localScale = Vector3.one;
        mTransform.localPosition = Vector3.zero;
        mButton = GetComponent<Button>();
        mImage = GetComponent<Image>();
        mImage.sprite = Data.Image;
        
        mButton.onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        Panel.Equip(Data);
    }
}
