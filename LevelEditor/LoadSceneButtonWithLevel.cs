using TMPro;
using Unity.FPS.Game;
using Unity.FPS.LevelEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Unity.FPS.UI
{
    public class LoadSceneButtonWithLevel : MonoBehaviour
    {
        public string SceneName = "";
        private MapData _mapData;
        [SerializeField] private TextMeshProUGUI _buttonName;

        public void Setup(MapData mapData)
        {
            _buttonName.text = mapData.MapName;
            _mapData = mapData;
        }
        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject
                && Input.GetButtonDown(GameConstants.k_ButtonNameSubmit))
            {
                LoadTargetScene();
            }
        }

        //Loads the base map and adds the MapData to be loaded at loading 
        public void LoadTargetScene()
        {
            SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive).completed += operation =>
            {
                FindObjectOfType<LoadSystem>().LoadMap(_mapData);
                SceneManager.UnloadSceneAsync(0);
            };
        }
    }
}