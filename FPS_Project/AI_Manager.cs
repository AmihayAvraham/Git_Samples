using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private OrbitalTargeting playerOrbit;

    [SerializeField] private List<AICharacter> _AIPrefab;
    [SerializeField] private List<AICharacter> _activeCharacters;
    [SerializeField] private Queue<AICharacter> _queuedCharacters;

    [SerializeField] private Transform[] _spawnPoints;

    [SerializeField] private int _activeBots;
    [SerializeField] private float _delaySpawn;

    public static AI_Manager Instance;

    private void Start()
    {
        Instance = this;
        _queuedCharacters = new Queue<AICharacter>();

        for (int i = 0; i < _activeBots; i++)
        {
            GetCharacter();
        }
    }

    public AICharacter GetCharacter()
    {
        if(_queuedCharacters.Count<=0)
        {
            AICharacter characterToAdd = Instantiate(_AIPrefab[Random.Range(0, _AIPrefab.Count)], transform);
            characterToAdd.Target = player;
            characterToAdd.currentTargetOrbit = playerOrbit;
            characterToAdd.GetComponent<Health>().OnDeath += OnCharacterDies;
            _queuedCharacters.Enqueue(characterToAdd);
        }

        AICharacter characterToSpawn = _queuedCharacters.Dequeue();
        _activeCharacters.Add(characterToSpawn);
        characterToSpawn.transform.position = GetSpawnLocation();
        return characterToSpawn;
    }

    public void OnCharacterDies(DamageType damage)
    {
        _activeCharacters.RemoveAt(0);

        Invoke("GetCharacter", _delaySpawn);
    }

    public void ReturnCharacter(AICharacter characterToReturn)
    {
        _activeCharacters.Remove(characterToReturn);
        _queuedCharacters.Enqueue(characterToReturn);
    }

    public Vector3 GetSpawnLocation()
    {
        int rnd = Random.Range(0, _spawnPoints.Length);
        return _spawnPoints[rnd].position;
    }
}
