using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Skill
{
    public string name;

    [Header("Effect")]
    public GameObject prefabSkill;
    public float delaySpawn;
    public Vector3 offsetSpwanSkill;
    public Transform positionSpawn;
    public MMFeedbacks feedbackOnStart;
    public MMFeedbacks feedbackOnSpawn;

    [Header("Character")]
    public string animTriggerParam;
    public float durationAnimationLock;
    public bool isInvulnerableWhileActivating;
    public bool applyRootMotion;
}

public class SkillManager : MonoBehaviour
{
    public List<Skill> skills;

    private Skill currentSkill;
    private Animator anim;
    private Health health;

    public bool IsActivatingSkill { get; private set; }

    private void Start()
    {
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();

        IsActivatingSkill = false;
    }

    public void StartSkill(string skillName)
    {
        foreach (Skill skill in skills)
        {
            if (skill.name.Equals(skillName))
            {
                ActivateSkill(skill);
                break;
            }
        }
    }

    private void ActivateSkill(Skill skill)
    {
        currentSkill = skill;
        
        IsActivatingSkill = true;

        if (currentSkill.isInvulnerableWhileActivating)
            health.Invulnerable = true;

        if (currentSkill.applyRootMotion)
            anim.applyRootMotion = true;

        anim.SetTrigger(currentSkill.animTriggerParam);

        if (currentSkill.feedbackOnStart)
            currentSkill.feedbackOnStart.PlayFeedbacks();

        Invoke("SpawnSkill", currentSkill.delaySpawn);
        Invoke("UnlockController", currentSkill.durationAnimationLock);
    }

    private void SpawnSkill()
    {
        if (currentSkill.prefabSkill != null)
        {
            GameObject obj = Instantiate(currentSkill.prefabSkill, GetSpawnPos(), GetSpawnRotation());
            AreaEffect areaEffect = obj.GetComponent<AreaEffect>();

            if (areaEffect)
                areaEffect.SetOwner(gameObject);
        }

        if (currentSkill.feedbackOnSpawn)
            currentSkill.feedbackOnSpawn.PlayFeedbacks();
    }

    private Vector3 GetSpawnPos()
    {
        if (currentSkill.positionSpawn != null)
        {
            return currentSkill.positionSpawn.position;
        }
        
        return transform.position + currentSkill.offsetSpwanSkill;
    }

    private Quaternion GetSpawnRotation()
    {
        if (currentSkill.positionSpawn != null)
        {
            return currentSkill.positionSpawn.rotation;
        }
        return Quaternion.identity;
    }

    private void UnlockController()
    {
        if (currentSkill.isInvulnerableWhileActivating)
            health.Invulnerable = false;

        if (currentSkill.applyRootMotion)
            anim.applyRootMotion = false;

        IsActivatingSkill = false;
    }
}
