using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEditor;

namespace Remuser
{
    enum TrackType
    {
        Animal1,
        Animal2,
        Animal3,
        Animal4,
    }
    public class TracksPrefabBuilder : EditorWindow
    {
        private string[] trackAnimal= new string[4];

        private string[] genreType = new string[6];
        private TrackUI[] trackParents = new TrackUI[4];
        private string sceneName;
        private AudioClip[] songArray;

        [MenuItem("Window/TracksBuilder")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<TracksPrefabBuilder>("TracksBuilder");
        }
        private void OnGUI()
        {
            GUILayout.Label("Tracks Settings", EditorStyles.boldLabel);
            sceneName = EditorGUILayout.TextField("Scene Name",sceneName);
            for (int i=0;i<genreType.Length;i++)
            {
                genreType[i] = EditorGUILayout.TextField("Track genre " + (i+1), genreType[i]);
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
                                trackGenre = "genre" + (k + 1);
                                break;
                            }
                        }

                        //string audioClipName = sceneName + " - " + trackType + " - " + trackGenre;
                        string audioClipName = "track_" + sceneName + "_" + trackType + "_" + trackGenre;
                        AudioClip curClip = FindClipInArray(songArray, audioClipName);
                        if (curClip == null||trackGenre==null)
                            Debug.Log(trackStyles[j].name + " Can't find " + audioClipName + " in songs array!");
                        else
                        {
                            trackStyles[j].SetAudio(curClip);
                            Debug.Log(audioClipName +" Success");
                        }
                    }
                }
            }
        }
        private AudioClip FindClipInArray(AudioClip[] songArray, string audioName)
        {
            foreach (var item in songArray)
            {
                if(item.name.ToLower().Contains(audioName.ToLower()))
                    return item;
               
            }

            return null;
        }
        
    }
}
