using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesCollider : MonoBehaviour
{
    public LayerMask enemyLayer;
    public string enemyTag;
    public Transform aim;
    public Transform defaultPos;
    public float aimSpeed;
    public bool rotateToTarget = false;
    public bool setTargetOutlined = false;
    public Transform rootRotation;

    public AICharacter myCharacter;

    public ITeamMember CurrentTargetTeam { get; private set; }
    public AICharacter CurrentAITarget { get; private set; }
    public Transform CurrentTarget { get; private set; }
    public Health TargetHealth { get; private set; }

    private AICharacter targetAI;

    public void Init(Team enemy)
    {
        ResetTarget();

        enemyTag = enemy.ToString();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals(enemyTag) && !other.transform.Equals(CurrentTarget))
        {
            TargetHealth = other.gameObject.GetComponent<Health>();
            
            if (TargetHealth != null)
            {
                CurrentTarget = other.transform;

                targetAI = other.gameObject.GetComponent<AICharacter>();
                if (targetAI != null)
                {
                    RemoveTargetFromCurrentAI();

                    CurrentTargetTeam = targetAI;
                    CurrentAITarget = targetAI;

                    if (setTargetOutlined)
                        CurrentAITarget.SetTargeted(true);
                }
                else
                {
                    CurrentTargetTeam = other.gameObject.GetComponent<PlayerCharacter>();
                }

                if (CurrentTargetTeam != null && myCharacter != null)
                    myCharacter.SetTarget(CurrentTargetTeam);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.Equals(CurrentTarget))
        {
            ResetTarget();
        }
    }

    private void RemoveTargetFromCurrentAI()
    {
        if (setTargetOutlined && CurrentAITarget != null)
            CurrentAITarget.SetTargeted(false);
    }

    public void ResetTarget()
    {
        RemoveTargetFromCurrentAI();

        CurrentAITarget = null;
        CurrentTarget = null;
        TargetHealth = null;
        CurrentTargetTeam = null;
    }

    private void FixedUpdate()
    {
        if (TargetHealth != null && TargetHealth.CurrentHealth <= 0)
        {
            ResetTarget();    
        }

        if (TargetHealth != null)
        {
            aim.position = Vector3.MoveTowards(aim.position, TargetHealth.CenterOfMass.position, aimSpeed);
        }
        else
        {
            aim.position = Vector3.MoveTowards(aim.position, defaultPos.position, aimSpeed);
        }

        if (rotateToTarget)
        {
            Quaternion newRotation = Quaternion.LookRotation(aim.position - rootRotation.position);
            newRotation.x = 0;
            rootRotation.rotation = newRotation;
        }
    }
}
