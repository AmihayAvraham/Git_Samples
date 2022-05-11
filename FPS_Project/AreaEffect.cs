using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : MonoBehaviour
{
    [Header("Effect")]
    [SerializeField]
    private float duration;
    [SerializeField]
    private float yOffset;

    [Header("Damage")]
    [SerializeField]
    private bool hasDamage;
    [SerializeField]
    private float damageDelay;
    [SerializeField]
    private float damageDuration;
    [SerializeField]
    private DamageOnTouch damage;
    [SerializeField]
    private MMFeedbacks feedbackOnDamageStart;

    public bool IsActive { get; private set; } = false;
    public bool IsDamageActive { get; private set; } = false;

    private float startTime;
    private bool damageActivated;

    public float Duration
    {
        get { return duration; }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        damageActivated = false;
        SetDamage(damageDelay == 0);
        startTime = Time.time;
        IsActive = true;

        Vector3 posEffect = transform.position;
        posEffect.y = 0 + yOffset;
        transform.position = posEffect;
    }

    public void SetOwner(GameObject owner)
    {
        if (damage != null)
            damage.Owner = owner;
    }

    private void FixedUpdate()
    {
        if (hasDamage)
        {
            if (!damageActivated && !IsDamageActive && Time.time - startTime >= damageDelay)
            {
                SetDamage(true);
            }
            if (IsDamageActive && Time.time - startTime >= damageDelay + damageDuration)
            {
                SetDamage(false);
            }
        }

        if (IsActive && Time.time - startTime >= duration)
        {
            Disable();
        }
    }

    public void Disable()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }

    public void SetDamage(bool value)
    {
        if (hasDamage)
        {
            damage.gameObject.SetActive(value);
            IsDamageActive = value;

            if (value)
            {
                damageActivated = true;

                if (feedbackOnDamageStart)
                    feedbackOnDamageStart.PlayFeedbacks();
            }
        }
    }
}
