using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Team1,
    Team2
}

public interface ITeamMember
{
    void Init(Team team);
    Team GetTeam();

    void SetTarget(ITeamMember target);
    ITeamMember GetTarget();

    Transform GetTransform();
    OrbitalTargeting GetOrbitalTargeting();
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PlayerCharacter player;

    [SerializeField] 
    private List<AICharacter> prefabsPlayerTeam;
    [SerializeField]
    private List<AICharacter> prefabsEnemies;
    [SerializeField]
    private AICharacter prefabsBoss;
    [SerializeField]
    private Transform[] playerSpawnPoints;
    [SerializeField] 
    private Transform[] enemySpawnPoints;
    [SerializeField]
    private Transform[] bossSpawnPoints;

    public int numOfPlayerTeamMembers;
    public int numOfEnemyTeamMembers;
    public bool includeBoss = false;

    public float delaySpawn;

    private List<ITeamMember> teamPlayer;
    private List<ITeamMember> teamEnemy;

    public static GameManager Instance;

    private bool isInitialized = false;

    private void Start()
    {
        Instance = this;

        teamPlayer = new List<ITeamMember>();
        teamEnemy = new List<ITeamMember>();

        if (numOfPlayerTeamMembers > 0 && prefabsPlayerTeam.Count > 0 && playerSpawnPoints.Length > 0)
        {
            for (int i = 0; i < numOfPlayerTeamMembers; i++)
            {
                AddTeam1Character();
            }
        }

        if (numOfEnemyTeamMembers > 0 && prefabsEnemies.Count > 0 && enemySpawnPoints.Length > 0)
        {
            for (int i = 0; i < numOfEnemyTeamMembers; i++)
            {
                AddTeam2Character();
            }
        }

        teamPlayer.Add(player);

        if (includeBoss && prefabsBoss != null)
            CreateCharacter(Team.Team2, GetSpawnLocation(bossSpawnPoints), prefabsBoss);

        SetInitialTargets();

        isInitialized = true;
    }

    public void CreateCharacter(Team team, Vector3 initPos, AICharacter prefab = null)
    {
        AICharacter characterPrefab = prefab;
        
        if (characterPrefab == null)
            characterPrefab = GetRandomPrefab((team == Team.Team1) ? prefabsPlayerTeam : prefabsEnemies);

        AICharacter characterToAdd = Instantiate(characterPrefab, transform);
        characterToAdd.Init(team);

        if (team == Team.Team1)
            teamPlayer.Add(characterToAdd);
        else 
            teamEnemy.Add(characterToAdd);

        characterToAdd.transform.position = initPos;

        if (isInitialized)
        {
            SetTarget(characterToAdd);
        }
    }

    private AICharacter GetRandomPrefab(List<AICharacter> pool)
    {
        return pool[Random.Range(0, pool.Count)];
    }

    public Vector3 GetSpawnLocation(Transform[] transforms)
    {
        int rnd = Random.Range(0, transforms.Length);
        return transforms[rnd].position;
    }

    private void SetInitialTargets()
    {
        foreach (ITeamMember member in teamPlayer)
        {
            SetTarget(member);
        }

        foreach (ITeamMember member in teamEnemy)
        {
            SetTarget(member);
        }
    }

    public void SetTarget(ITeamMember character)
    {
        if (teamPlayer.Count == 0 || teamEnemy.Count == 0)
            return;

        if (character.GetTeam() == Team.Team1)
            character.SetTarget(teamEnemy[Random.Range(0, teamEnemy.Count)]);
        else
            character.SetTarget(teamPlayer[Random.Range(0, teamPlayer.Count)]);
    }

    public void OnCharacterDeath(ITeamMember teamMember)
    {
        if (teamMember.GetTeam() == Team.Team1)
        {
            teamPlayer.Remove(teamMember);
            Invoke("AddTeam1Character", delaySpawn);
        }
        else
        {
            teamEnemy.Remove(teamMember);
            Invoke("AddTeam2Character", delaySpawn);
        }
    }

    private void AddTeam1Character()
    {
        CreateCharacter(Team.Team1, GetSpawnLocation(playerSpawnPoints));
    }

    private void AddTeam2Character()
    {
        CreateCharacter(Team.Team2, GetSpawnLocation(enemySpawnPoints));
    }
}
