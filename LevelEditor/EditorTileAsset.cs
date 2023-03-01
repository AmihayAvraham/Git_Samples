using UnityEngine;

namespace Unity.FPS.LevelEditor
{
    [CreateAssetMenu(fileName = "Tile_Data", menuName = "Level_Editor/TilesAsset", order = 0)]
    public class EditorTileAsset : ScriptableObject
    {
        public TileData[] TilesPrefabs;

        [System.Serializable]
        public struct TileData
        {
            public string TileName;
            public string TileID;
            public GameObject TilePrefab;
            public GameObject TileLoadPrefab;
            public Sprite TileImage;
        }
    }
}
