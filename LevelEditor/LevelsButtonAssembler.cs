using System.IO;
using Unity.FPS.LevelEditor;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class LevelsButtonAssembler : MonoBehaviour
    {
        [SerializeField] private GameObject _buttonPrefab;

        private void Start()
        {
            AssembleButtons();
        }

        //Creates the button Array to represent the maps located in the maps folder
        private void AssembleButtons()
        {
            string path = Application.persistentDataPath;
            string mapsPath = Path.Combine(path, "Maps");
            string[] levels= Directory.GetFiles(mapsPath);

            Transform trans = transform;
            foreach (string level in levels)
            {
                MapData mapData = JsonUtility.FromJson<MapData>(File.ReadAllText(level));
                GameObject buttonObject = Instantiate(_buttonPrefab, trans);
                buttonObject.GetComponent<LoadSceneButtonWithLevel>().Setup(mapData);
            }
        }
        
    }
}
