using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Presets;

namespace Remuser
{
    public class PrefabBuilder : EditorWindow
    {
        private string[] animationList = new string[4];

        [MenuItem("Window/PrefabBuilder")]
        public static void ShowWindow()
        {
            GetWindow<PrefabBuilder>("PrefabBuilder");
        }
        private void OnGUI()
        {
            GUILayout.Label("Animation List", EditorStyles.boldLabel);
            
            for (int i=0;i<animationList.Length;i++)
            {
                animationList[i] = EditorGUILayout.TextField("Animation " + (i+1), animationList[i]);
            }

            if (GUILayout.Button("Create Prefabs"))
            {
                var models = Selection.objects;
                Debug.Log(models.Length + " Prefabs to create");
                for (int i=0;i<models.Length;i++)
                {
                    GameObject obj = (GameObject)models[i];
                    if (obj != null)
                    {
                        BuildPrefab(obj);
                        Debug.Log("Created Prefab: " + models[i]);
                    }
                }
            }
        }
        
        public void BuildPrefab(GameObject g)
        {
            GameObject prefab = g;
            
            string localPath = "Assets/Preset/Prefabs/" + prefab.name + ".prefab";
            GameObject instanceRoot = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            
            

            //AnimatorController animatorToCreate = AnimatorController.CreateAnimatorControllerAtPath(newAssetName);
            //AnimatorOverrideController animatorToCreate = new AnimatorOverrideController();
            
            AnimatorController animatorPreset =
                (AnimatorController) AssetDatabase.LoadMainAssetAtPath("Assets/Preset/Base/BaseController.controller");
            Debug.Log(animatorPreset.name);
            AnimatorOverrideController animatorToCreate = new AnimatorOverrideController(animatorPreset);

            string savePath = "Assets/Preset/Animators/";
            savePath = savePath.Substring(0, savePath.LastIndexOf('/') + 1);


            string controllerName = g.name;

            string newAssetName = savePath + controllerName + ".controller";

            AssetDatabase.CreateAsset(animatorToCreate, newAssetName);

            AssetDatabase.SaveAssets();
            
            string assetPath = "Assets/Preset/Models/" + prefab.name + ".fbx";
            Debug.Log(animatorToCreate.animationClips.Length);
            PrepareController(animatorToCreate, assetPath);

            Animator anim = instanceRoot.GetComponent<Animator>();
            anim.runtimeAnimatorController =  (AnimatorOverrideController) AssetDatabase.LoadMainAssetAtPath(newAssetName);;
            var variantRoot = PrefabUtility.SaveAsPrefabAsset((GameObject) instanceRoot, localPath);
            //DestroyImmediate(instanceRoot);
        }


        void PrepareController(AnimatorOverrideController controller, string assetPath)
        {
            var obj = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            
            var clipOverrides = new AnimationClipOverrides(controller.overridesCount);
            controller.GetOverrides(clipOverrides);
            List<AnimationClip> animationClips = new List<AnimationClip>();
            for (int i = 0; i < obj.Length; i++)
            {
                AnimationClip clip = obj[i] as AnimationClip;
                if (clip != null)
                {
                    animationClips.Add(clip);
                }
            }
            for (int j = 0; j < animationList.Length; j++)
                {
                    //var state = controller.layers[0].stateMachine.states.FirstOrDefault(s => s.state.name.Equals(animationList[j])).state;
                    var state = animationList[j];
                    if (state == null)
                    {
                        Debug.LogError("Couldn't get the state!" + animationList[j]);
                    }
                    else
                    {
                        string stateName = state;
                        stateName = stateName.Replace(" ", String.Empty);
                        stateName = stateName.Replace("_", String.Empty);
                        Debug.Log(stateName);
                        for (int i = 0; i < animationClips.Count; i++)
                        {
                            string animationName = animationClips[i].name.ToLower();
                            animationName = animationName.Replace(" ", String.Empty);
                            animationName = animationName.Replace("_", String.Empty);
                            if (animationName.Contains(stateName))
                            {
                                AnimationClip clip = animationClips[i];
                                clipOverrides[controller.animationClips[j].name] = clip;
                                //controller.animationClips[j] = clip;
                                break;
                            }
                        }
                        
                        controller.ApplyOverrides(clipOverrides);
                    }
                }
        }
        
        public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
        {
            public AnimationClipOverrides(int capacity) : base(capacity) {}

            public AnimationClip this[string name]
            {
                get { return this.Find(x => x.Key.name.Equals(name)).Value; }
                set
                {
                    int index = this.FindIndex(x => x.Key.name.Equals(name));
                    if (index != -1)
                        this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
                }
            }
        }

    }
}
