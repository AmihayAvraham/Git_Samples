using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DeathImpact
{
    public DamageType deathType;
    public Material material;
    public MMFeedbacks feedbackOnDeath;

    public bool toDissolve;
    public string dissolveParameterName;
    public float dissolveSpeed;
    public AnimationCurve dissolveCurve;
    public float dissolveStartVal;
    public float dissolveEndVal;
}

public class RagdollOnDeath : MonoBehaviour
{
    [SerializeField]
    private float forceMultipleMax = 130;
    [SerializeField]
    private float forceMultipleMin = 100;
    [SerializeField]
    private Rigidbody[] bodyPartsToHit;
    [SerializeField]
    private float durationAlive;
    [SerializeField]
    private Vector3 impactOnStart;

    [SerializeField]
    private List<DeathImpact> deathImpacts;
    [SerializeField]
    private Renderer[] renderers;

    private bool isDissolveStart = false;
    private float currentDissolve;
    private float dissolveTime;

    public DeathImpact CurrentDeathImpact { get; private set; }

    public void Init(Vector3 impactToAdd, DamageType deathType)
    {
        Invoke("Disable", durationAlive);

        AddImpact(transform.rotation * impactOnStart);
        AddImpact(impactToAdd);

        if (GetDeathImpact(deathType))
        {
            SetMaterialsByDeath();

            if (CurrentDeathImpact.feedbackOnDeath)
                CurrentDeathImpact.feedbackOnDeath.PlayFeedbacks();
        }
    }

    private void AddImpact(Vector3 impact)
    {
        if (impact != Vector3.zero)
        {
            bodyPartsToHit[UnityEngine.Random.Range(0, bodyPartsToHit.Length)].
                AddForce(impact * UnityEngine.Random.Range(forceMultipleMin, forceMultipleMax), ForceMode.Acceleration);
        }
    }

    private bool GetDeathImpact(DamageType deathType)
    {
        if (deathImpacts == null || deathImpacts.Count == 0)
            return false;

        foreach (DeathImpact deathImpact in deathImpacts)
        {
            if (deathImpact.deathType == deathType)
            {
                CurrentDeathImpact = deathImpact;

                return true;
            }
        }

        return false;
    }

    private void FixedUpdate()
    {
        UpdateDissolve();
    }

    private void SetMaterialsByDeath()
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material = CurrentDeathImpact.material;

            if (CurrentDeathImpact.toDissolve)
            {
                currentDissolve = CurrentDeathImpact.dissolveStartVal;
                dissolveTime = 0;
                isDissolveStart = true;
            }
        }
    }

    private void UpdateDissolve()
    {
        if (CurrentDeathImpact.toDissolve && isDissolveStart && currentDissolve <= CurrentDeathImpact.dissolveEndVal)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetFloat(CurrentDeathImpact.dissolveParameterName, currentDissolve);
            }

            dissolveTime += Time.deltaTime;
            currentDissolve = Mathf.Lerp(currentDissolve, CurrentDeathImpact.dissolveEndVal, CurrentDeathImpact.dissolveCurve.Evaluate(dissolveTime));
        }
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
