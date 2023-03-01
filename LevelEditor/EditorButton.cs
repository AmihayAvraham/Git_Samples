using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.LevelEditor
{
    public class EditorButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _tileNameText;
        [SerializeField] private TextMeshProUGUI _tileNumberText;
        [SerializeField] private Image _tileImage;
        [SerializeField] private int _tileNumber;
        private BuildSystem _buildSystem;

        public void Setup(BuildSystem buildSystem, int number, string tileName,Sprite image)
        {
            _buildSystem = buildSystem;
            _tileNumber = number;
            _tileNumberText.text = (_tileNumber + 1).ToString();
            _tileNameText.text = tileName;
            _tileImage.sprite = image;
        }

        public void SpawnTile()
        {
            _buildSystem.SpawnTile(_tileNumber); 
        }
    }
}
