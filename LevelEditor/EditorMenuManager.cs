using TMPro;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.LevelEditor
{
    public class EditorMenuManager : MonoBehaviour
    {
        [Tooltip("Root GameObject of the menu used to toggle its activation")]
        public GameObject MenuRoot;

        [Tooltip("Master volume when menu is open")] [Range(0.001f, 1f)]
        public float VolumeWhenMenuOpen = 0.5f;
        
        [Tooltip("GameObject for the controls")]
        public GameObject ControlImage;

        [Tooltip("Map name to save")]
        public TMP_InputField MapNameToSave;
        
        [Tooltip("Build system reference")]
        public BuildSystem BuildSystem;
        
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (ControlImage.activeSelf)
                {
                    ControlImage.SetActive(false);
                    return;
                }

                SetPauseMenuActivation(!MenuRoot.activeSelf);
            }
        }

        public void ClosePauseMenu()
        {
            SetPauseMenuActivation(false);
        }

        void SetPauseMenuActivation(bool active)
        {
            MenuRoot.SetActive(active);

            if (MenuRoot.activeSelf)
            {
                AudioUtility.SetMasterVolume(VolumeWhenMenuOpen);
            }
            else
            {
                AudioUtility.SetMasterVolume(1);
            }

        }

        public void OnShowControlButtonClicked(bool show)
        {
            ControlImage.SetActive(show);
        }

        public void OnSaveMap()
        {
            if (string.IsNullOrEmpty(MapNameToSave.text))
            {
                return;
            }
            
            BuildSystem.SaveMap(MapNameToSave.text);
            
        }
    }
}