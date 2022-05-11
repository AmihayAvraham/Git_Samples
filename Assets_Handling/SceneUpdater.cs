using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Remuser
{
    public class SceneUpdater : EditorWindow
    {
        private string path;
        private string sceneName;

        private List<BackGroundImageClass> _resourceImagesBG;
        private List<LabeledButtonReferenceClass> _resourceImagesButtons;

        private string[] trackAnimal = new string[4];

        private string[] genreType = new string[6];
        private TrackUI[] trackParents = new TrackUI[4];

        private AudioClip[] songArray;

        private static readonly Regex regex = new Regex(@"^\d+$");

        [MenuItem("Window/SceneUpdater")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<SceneUpdater>("SceneUpdater");
        }

        private void OnGUI()
        {
            GUILayout.Label("Scene Settings", EditorStyles.boldLabel);
            sceneName = EditorGUILayout.TextField("Scene Name", sceneName);
            path = EditorGUILayout.TextField("Scene Path", path);

            if (GUILayout.Button("Build Background"))
            {

                if (Selection.activeGameObject.GetComponent<SceneUpdaterReferenceScript>() == null)
                    return;

                SceneUpdaterReferenceScript referenceScript =
                    Selection.activeGameObject.GetComponent<SceneUpdaterReferenceScript>();

                if (_resourceImagesBG == null)
                {
                    _resourceImagesBG = new List<BackGroundImageClass>();
                }

                if (_resourceImagesButtons == null)
                {
                    _resourceImagesButtons = new List<LabeledButtonReferenceClass>();
                }
                _resourceImagesBG.Clear();
                _resourceImagesButtons.Clear();
                string[] resourcePaths = Directory.GetFiles(path + "/Image", "*.png");
                List<string> labelsList = referenceScript.GetLabelesList();

                foreach (string filePath in resourcePaths)
                {
                    Sprite resource = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);

                    if (resource != null)
                    {
                        if (resource.name.Contains("BG"))
                        {
                            BackGroundImageClass imageToAdd = new BackGroundImageClass();
                            imageToAdd.SourceImage = resource;
                            string[] splitName = resource.name.Split('_');

                            foreach (var o in splitName)
                            {
                                if (regex.IsMatch(o))
                                {
                                    imageToAdd.SortingLayer = int.Parse(o);
                                }
                            }

                            _resourceImagesBG.Add(imageToAdd);
                        }
                        /*else if (resource.name.Contains("Buttons"))
                        {
                            LabeledButtonClass buttonToAdd = new LabeledButtonClass();
                            buttonToAdd.SourceImage = resource;
                            
                            string[] splitName = resource.name.Split('_');
                           
                            foreach (var o in splitName)
                            {
                                for (int i = 0; i < labelsList.Count; i++)
                                {
                                    if (o==labelsList[i])
                                    {
                                        buttonToAdd.Label = o;
                                    }
                                }
                            }
                            
                            _resourceImagesButtons.Add(buttonToAdd);
                            Debug.Log(" added to Buttons: " + resource.name);
                        }*/
                    }
                }

                referenceScript.SetBackGroundSpriteList(_resourceImagesBG);

                referenceScript.UpdateScene();

                /*Debug.Log(path + " : " + resources.Length);
                foreach (var resource in resources)
                {
                    if (resource is Sprite)
                    {
                        _resourceImages.Add((Sprite)resource);
                        Debug.Log(" added: " + resource.name);
                    }
                }*/
                /*
                songArray = Selection.activeGameObject.GetComponent<TracksManager>().GetAudioClips();
                TrackUI[] trackUIs = Selection.activeGameObject.GetComponentsInChildren<TrackUI>();
                for (int i=0;i<trackParents.Length;i++)
                {
                    TrackType curTrackType = (TrackType) i;
                    String trackType = curTrackType.ToString();
                    for (int j = 0; j < trackUIs.Length; j++)
                    {
                        if (trackUIs[j].name == trackAnimal[i])
                        {
                            trackParents[i] = trackUIs[j];
                        }
                    }

                    TrackStyle[] trackStyles = trackParents[i].GetComponentsInChildren<TrackStyle>();

                    for (int j = 0; j < trackStyles.Length; j++)
                    {
                        String trackGenre = null;
                        for (int k = 0; k < trackStyles.Length; k++)
                        {
                            string curGenre = genreType[k];
                            if (trackStyles[j].name.ToLower().Contains(curGenre.ToLower()))
                            {
                                trackGenre = "GENRE " + (k + 1);
                                break;
                            }
                        }

                        string audioClipName = sceneName + " - " + trackType + " - " + trackGenre;
                        AudioClip curClip = FindClipInArray(songArray, audioClipName);
                        if (curClip == null||trackGenre==null)
                            Debug.Log(trackStyles[j].name + " Can't find " + audioClipName + " in songs array!");
                        else
                        {
                            trackStyles[j].SetAudio(curClip);
                            
                        }
                    }*/

            }

            GUILayout.Label("Tracks Settings", EditorStyles.boldLabel);
            sceneName = EditorGUILayout.TextField("Scene Name", sceneName);
            for (int i = 0; i < genreType.Length; i++)
            {
                genreType[i] = EditorGUILayout.TextField("Track genre " + (i + 1), genreType[i]);
            }

            trackAnimal[0] = "Buffalo";
            trackAnimal[1] = "Elephant";
            trackAnimal[2] = "Giraffe";
            trackAnimal[3] = "Bird";

            if (GUILayout.Button("Sort Tracks"))
            {

                if (Selection.activeGameObject.GetComponent<TracksManager>() == null)
                    return;

                songArray = Selection.activeGameObject.GetComponent<TracksManager>().GetAudioClips();
                TrackUI[] trackUIs = Selection.activeGameObject.GetComponentsInChildren<TrackUI>();
                for (int i = 0; i < trackParents.Length; i++)
                {
                    TrackType curTrackType = (TrackType) i;
                    String trackType = curTrackType.ToString();
                    for (int j = 0; j < trackUIs.Length; j++)
                    {
                        if (trackUIs[j].name == trackAnimal[i])
                        {
                            trackParents[i] = trackUIs[j];
                        }
                    }

                    TrackStyle[] trackStyles = trackParents[i].GetComponentsInChildren<TrackStyle>();

                    for (int j = 0; j < trackStyles.Length; j++)
                    {
                        String trackGenre = null;
                        for (int k = 0; k < trackStyles.Length; k++)
                        {
                            string curGenre = genreType[k];
                            if (trackStyles[j].name.ToLower().Contains(curGenre.ToLower()))
                            {
                                trackGenre = "genre" + (k + 1);
                                break;
                            }
                        }

                        string audioClipName = "track_" + sceneName + "_" + trackType + "_" + trackGenre;
                        AudioClip curClip = FindClipInArray(songArray, audioClipName);
                        if (curClip == null || trackGenre == null)
                            Debug.Log(trackStyles[j].name + " Can't find " + audioClipName + " in songs array!");
                        else
                        {
                            trackStyles[j].SetAudio(curClip);
                            Debug.Log(audioClipName + " Success");
                        }
                    }
                }

            }


        }

        private AudioClip FindClipInArray(AudioClip[] songArray, string audioName)
        {
            foreach (var item in songArray)
            {
                if (item.name.ToLower().Contains(audioName.ToLower()))
                    return item;

            }

            return null;
        }
        /*
        }*/
    }
}
