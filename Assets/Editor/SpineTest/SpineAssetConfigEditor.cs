using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using Spine.Unity.Editor;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SpineAssetConfig))]
public class SpineAssetConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpineAssetConfig config = target as SpineAssetConfig;
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("生成动画文件"))
        {
            if (config != null && config.Asset != null)
            {
                string goName = null;
                while (config.SpineRoot.childCount > 0)
                {
                    goName = config.SpineRoot.GetChild(0).gameObject.name;
                    DestroyImmediate(config.SpineRoot.GetChild(0).gameObject);
                }

                Transform trans = EditorInstantiation.InstantiateSkeletonAnimation(config.Asset).transform;
                trans.SetParent(config.SpineRoot);
                trans.localPosition = Vector3.zero;
                trans.gameObject.AddComponent<CharacterBaseController>();
                trans.gameObject.GetComponent<MeshRenderer>().sortingLayerName = "Character";
                // trans.gameObject.AddComponent<ShadowCaster2D>();

                SkeletonAnimation spine = trans.gameObject.GetComponent<SkeletonAnimation>();
                float scale = spine.skeletonDataAsset.scale;
                float height = spine.skeleton.Data.Height * scale;
                trans.localScale = new Vector3(10 / height, 10 / height, 1);
                
                if (goName != trans.gameObject.name)
                {
                    config.EquipConfigList.Clear();
                    config.DefaultSkinArray = null;
                    config.IdleAnim = null;
                    config.WalkAnim = null;
                    config.RunAnim = null;
                    config.JumpAnim = null;
                    config.ShootAnim = null;
                    config.Skill1Anim = null;
                    config.Skill2Anim = null;
                    config.Skill3Anim = null;
                    config.ShootBone = null;
                }
            }
            else
                Debug.LogError("SpineTestPanelController组件的Asset为null");
        }

        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("显示默认皮肤"))
        {
            if (config != null)
            {
                SkeletonAnimation spine = config.SpineRoot.GetComponentInChildren<SkeletonAnimation>();
                if (spine != null && config.DefaultSkinArray != null && config.DefaultSkinArray.Length > 0)
                {
                    Skin skin = new Skin("temple");
                    spine.initialSkinName = config.DefaultSkinArray[0];

                    foreach (var skinName in config.DefaultSkinArray)
                        skin.AddAttachments(spine.Skeleton.Data.FindSkin(skinName));
                    spine.Skeleton.SetSkin(skin);
                    spine.Skeleton.SetSlotsToSetupPose();
                    spine.Update(0f);
                    spine.LateUpdate();
                }
            }
        }
        EditorGUILayout.EndHorizontal();
       
    }
}
