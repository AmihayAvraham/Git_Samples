using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, ITeamMember
{
    [SerializeField]
    private OrbitalTargeting orbit;
    [SerializeField]
    private EnemiesCollider enemiesCollider;

    public OrbitalTargeting GetOrbitalTargeting()
    {
        return orbit;
    }

    public ITeamMember GetTarget()
    {
        return enemiesCollider.CurrentTargetTeam;
    }

    public Team GetTeam()
    {
        return Team.Team1;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Init(Team team)
    {
        
    }

    public void SetTarget(ITeamMember target)
    {
    }
}
