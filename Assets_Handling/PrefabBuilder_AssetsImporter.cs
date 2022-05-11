using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Presets;
using UnityEngine;

namespace Remuser
{

    public class PrefabBuilderAssetsImporter : AssetPostprocessor
    {
        private bool _isWrong = false;
        void OnPreprocessTexture()
        {
            if (!IsAssetNew(typeof(Texture2D)))
                return;

            TextureImporter importer = (TextureImporter)assetImporter;
            string lcAssetPatch = assetImporter.assetPath.ToLower();


            if (lcAssetPatch.Contains("/preset/textures/"))
            {
                Preset preset = (Preset) AssetDatabase.LoadMainAssetAtPath("Assets/Preset/Base/BaseTexture.preset");
                preset.ApplyTo(importer);
            }
        }
        void OnPostprocessTexture(Texture2D texture)
        {
            TextureImporter importer = (TextureImporter)assetImporter;
            string lcAssetPatch = assetImporter.assetPath.ToLower();

            if (lcAssetPatch.Contains("/preset/textures/"))
            {
                string materialName = assetImporter.assetPath;
                materialName = materialName.Remove(0, 23);
                for (int i = 0; i < 4; i++)
                {
                    materialName = materialName.Remove(materialName.Length - 1);
                }

                Material newMaterial = new Material(Shader.Find("Unlit/Texture"));
                Preset preset = (Preset) AssetDatabase.LoadMainAssetAtPath("Assets/Preset/Base/BaseMaterial.preset");
                preset.ApplyTo(newMaterial);

                Texture textureToSet = (Texture) AssetDatabase.LoadMainAssetAtPath(assetImporter.assetPath);
                newMaterial.mainTexture = textureToSet;
                Debug.Log(textureToSet);

                string savePath = "Assets/Preset/Materials/";
                savePath = savePath.Substring(0, savePath.LastIndexOf('/') + 1);

                string newAssetName = savePath + materialName + ".mat";

                AssetDatabase.CreateAsset(newMaterial, newAssetName);

                AssetDatabase.SaveAssets();
            }
        }
        

        void OnPreprocessModel()
        {
            if (!IsAssetNew(typeof(Mesh)))
                return;

            ModelImporter importer = (ModelImporter)assetImporter;
            string lcAssetPatch = assetImporter.assetPath.ToLower();
            
            if (lcAssetPatch.Contains("/preset/models/"))
            {
                string modelName = assetImporter.assetPath;
                modelName = modelName.Remove(0, 23);
                for (int i = 0; i < 4; i++)
                {
                    modelName = modelName.Remove(modelName.Length - 1);
                }
                /*Preset preset = (Preset) AssetDatabase.LoadMainAssetAtPath("Assets/Preset/Base/BaseModel.preset");
                preset.ApplyTo(importer);*/

                importer.globalScale = 40;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;

                PrepareMaterial(modelName);
                importer.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnTextureName, 
                    ModelImporterMaterialSearch.RecursiveUp);
            }
        }

        private void OnPostprocessModel(GameObject g)
        {
            if (_isWrong)
             AssetDatabase.DeleteAsset(assetPath);
            
            ModelImporter importer = assetImporter as ModelImporter;
            string lcAssetPatch = assetImporter.assetPath.ToLower();

            if (lcAssetPatch.Contains("/preset/models/"))
            {
                var defaultClipAnimations = importer.defaultClipAnimations;
                Debug.Log(importer.clipAnimations.Length);
               foreach (var clipAnimation in defaultClipAnimations)
                {
                    if (clipAnimation.name.Contains("loop") ||
                        clipAnimation.name.Contains("song"))
                    {
                        clipAnimation.loop = true;
                        clipAnimation.loopTime = true;
                    }
                }
               importer.clipAnimations = defaultClipAnimations;
               importer.SaveAndReimport();
            }
        }

        void PrepareMaterial(string name)
        {
            string animal=null;
            string genre=null;

            if (name.Contains("bird"))
                animal = "bird";
            else if(name.Contains("giraffe"))
                animal = "giraffe";
            else if (name.Contains("elephant"))
                animal = "elephant";
            else if(name.Contains("bufallo"))
                animal = "bufallo";
            
            if (name.Contains("neutral"))
                genre = "neutral";
            else if(name.Contains("bossa_nova"))
                genre = "bossa_nova";
            else if (name.Contains("jazz"))
                genre = "jazz";
            else if(name.Contains("bit"))
                genre = "8bit";
            else if (name.Contains("folk"))
                genre = "folk";
            else if(name.Contains("india"))
                genre = "india";

            string materialPath = "Assets/Preset/Materials/" + animal + "_TEX_" + genre +".mat";
            Material materialToSet = (Material) AssetDatabase.LoadMainAssetAtPath(materialPath);
            if (materialToSet == null)
            {
                Debug.Log("Couldn't find material:" + materialPath);
                _isWrong = true;
                return;
            }

            if (materialToSet.mainTexture == null)
            {
                string texturePath = "Assets/Preset/Textures/" + animal + "_TEX_" + genre + ".png";
                materialToSet.mainTexture = (Texture) AssetDatabase.LoadMainAssetAtPath(texturePath);
            }
        }

        private bool IsAssetNew(System.Type type)
        {
            return !AssetDatabase.LoadAssetAtPath(assetImporter.assetPath, type);
        }
    }
}