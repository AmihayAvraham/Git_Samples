using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Unity.FPS.LevelEditor
{
    //Object class to represent save data from Editor for an object 
    [Serializable]
    public struct ObjectData
    {
        public string ObjectID;
        public Vector3 Position;
        public Vector3 Rotation;
    }
    
    //Map class to represent save data from Editor for a level
    [Serializable]
    public struct MapData
    {
        public string MapName;
        public ObjectData[] Objects;
    }
    
    //Responsible for handling the Editor objects spawning and placing
    public class BuildSystem : MonoBehaviour
    {
        [SerializeField] private LayerMask _placeableLayer;
        [SerializeField] private EditorTileAsset _editorTileAsset;
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _buttonPrefab;
        [SerializeField] private Transform _levelObjectsParent;
        [SerializeField] private Transform _buttonsParent;
        
        
        private GameObject _currentGameObject;
        private Transform _currentGameObjectTransform;
        private string _currentObjectID;
        private Vector3 _rotation;
        
        
        private List<ObjectData> _mapObjects;
        
        private bool _doRaycast;
        
      
        
        private void Start()
        {
            _mapObjects = new List<ObjectData>();
            
            //Generates UI buttons for the available build objects from the EditorTileAsset file
            for (int i = 0; i < _editorTileAsset.TilesPrefabs.Length; i++)
            {
                GameObject buttonToSetup = Instantiate(_buttonPrefab, _buttonsParent);
                buttonToSetup.GetComponent<EditorButton>().Setup(this,i,_editorTileAsset.TilesPrefabs[i].TileName,_editorTileAsset.TilesPrefabs[i].TileImage);
            }
        }

        private void Update()
        {
            if (_currentGameObject != null)
            {
                //HandlePlacement and HandleRotate handles the position and rotation of the level object
                HandlePlacement();
                HandleRotate();
                
                //Places and records the stats of the current build object
                if (Input.GetMouseButtonDown(0))
                {
                    ObjectData objectToAdd = new ObjectData();
                    objectToAdd.Position = _currentGameObjectTransform.position;
                    objectToAdd.Rotation = _currentGameObjectTransform.rotation.eulerAngles;

                    objectToAdd.ObjectID = _currentObjectID;
                    _mapObjects.Add(objectToAdd);
                    _currentGameObject = null;
                }
                
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Destroy(_currentGameObject);  
                }
            }
            
            if(_currentGameObject==null)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)){ SpawnTile(0);}
                if (Input.GetKeyDown(KeyCode.Alpha2)){ SpawnTile(1);}
                if (Input.GetKeyDown(KeyCode.Alpha3)){ SpawnTile(2);}
                if (Input.GetKeyDown(KeyCode.Alpha4)){ SpawnTile(3);}
                if (Input.GetKeyDown(KeyCode.Alpha5)){ SpawnTile(4);}
                if (Input.GetKeyDown(KeyCode.Alpha6)){ SpawnTile(5);}
                if (Input.GetKeyDown(KeyCode.Alpha7)){ SpawnTile(6);}
                if (Input.GetKeyDown(KeyCode.Alpha8)){ SpawnTile(7);}
            }
        }

        //Spawns new level object by the requested number
        public void SpawnTile(int num)
        {
            if (_currentGameObject == null)
            {
                _currentGameObject = Instantiate(_editorTileAsset.TilesPrefabs[num].TilePrefab, _levelObjectsParent);
                _currentGameObjectTransform = _currentGameObject.transform;
                _currentObjectID = _editorTileAsset.TilesPrefabs[num].TileID;
            }
        }

        private void HandlePlacement()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit,Single.PositiveInfinity, _placeableLayer))
            {
                _currentGameObjectTransform.position = hit.point;
            }
        }
        
        private void HandleRotate()
        {
            _rotation.y+=45 * Input.mouseScrollDelta.y;
            _currentGameObjectTransform.rotation = Quaternion.Euler(_rotation);
        }
        
        //Record and save the level data to json file on the persistentDataPath
        public void SaveMap(string mapName)
        {
            MapData mapData = new MapData();
            mapData.Objects = new ObjectData[_mapObjects.Count];
            mapData.MapName = mapName;
            
            for (int i = 0; i < _mapObjects.Count; i++)
            {
                mapData.Objects[i].Position = _mapObjects[i].Position;
                mapData.Objects[i].Rotation = _mapObjects[i].Rotation;
                mapData.Objects[i].ObjectID = _mapObjects[i].ObjectID;
            }
            
            string json = JsonUtility.ToJson(mapData);
            
            string path = Application.persistentDataPath;
            string mapsPath = Path.Combine(path, "Maps");
            if (!Directory.Exists(mapsPath))
            {
                Directory.CreateDirectory(mapsPath);
            }
            string filePath = Path.Combine(mapsPath, $"{mapData.MapName}.json");
            File.WriteAllText(filePath,json);
        }
    }
  
}
