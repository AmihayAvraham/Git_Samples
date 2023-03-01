using UnityEngine;

namespace Unity.FPS.LevelEditor
{
    public class LoadSystem : MonoBehaviour
    {
        [SerializeField] private EditorTileAsset _editorTileAsset;
        [SerializeField] private Transform _objectsParent;

        //Instantiates prefabs from the LOAD _editorTileAsset by the ID's saved from the SAVE assets at the Buildsystem 
        //The prefabs are different cause they act differently on play and don't require all functions when in Editor mode
        public void LoadMap(MapData mapData)
        {
            foreach (var mapObject in mapData.Objects)
            {
                Transform objectToPlace = Instantiate(GetTileByID(mapObject.ObjectID),_objectsParent).transform;
                objectToPlace.position = mapObject.Position;
                objectToPlace.rotation = Quaternion.Euler(mapObject.Rotation);
            }
        }
        private GameObject GetTileByID(string id)
        {
            for (int i = 0; i < _editorTileAsset.TilesPrefabs.Length; i++)
            {
                if (id == _editorTileAsset.TilesPrefabs[i].TileID)
                {
                    return _editorTileAsset.TilesPrefabs[i].TileLoadPrefab;
                }
            }

            return null;
        }
    }
}
