using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using RootMotion.FinalIK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AimManager : GenericBehaviour
{ 
    [SerializeField]
    public Transform rootUpperBody;
    [SerializeField]
    private EnemiesCollider enemiesCollider;
    [SerializeField]
    public Weapon initialWeapon;
    [SerializeField]
    private string reloadAnimBool;
    [SerializeField]
    private string shotAnimTrigger;
    [SerializeField]
    private string aimAnimBool;

    [SerializeField]
    private bool toShowHud = true;
    [SerializeField]
    private TMP_Text txtAmmo;
    [SerializeField]
    private Image imageReload;
    [SerializeField]
    private GameObject objTarget;
    [SerializeField]
    private bool canReload = true;

    [Header("Crosshair")]
    [SerializeField]
    private SimpleCrosshair targetCrosshair;
    [SerializeField]
    private int crosshairGapNotOnTarget;
    [SerializeField]
    private int crosshairGapOnTarget;
    [SerializeField]
    private float crosshairGapSpeed;

    public ArmIK ArmIK { get; set; }
    public bool IsWeaponEquiped { get; private set; }
    public Weapon CurrentWeapon { get; private set; }
    public bool IsReloading { get; private set; }

    public EnemiesCollider EnemiesCollider 
    { 
        get
        {
            return enemiesCollider;
        }
    }

    private float reloadStart;
    private MoveBehaviour moveBehaviour;
    private Health health;
    private Animator anim;
    private SuperSkillManager superSkillManager;
    private SkillManager skillManager;
    private ThrowManager throwManager;
    private float currentGap;
    private float reloadStatus;

    // Start is called before the first frame update
    void Start()
    {
        moveBehaviour = GetComponent<MoveBehaviour>();
        health = GetComponent<Health>();
        throwManager = GetComponent<ThrowManager>();
        skillManager = GetComponent<SkillManager>();
        anim = GetComponent<Animator>();

        IsReloading = false;
        SetWeapon(initialWeapon);

        if (behaviourManager)
            behaviourManager.SubscribeBehaviour(this);
        //behaviourManager.RegisterDefaultBehaviour(this.behaviourCode);

        UpdateWeaponUI();
        UpdateAimUI();
    }

    public void UnequipCurrentWeapon()
    {
        toShowHud = false;
        IsWeaponEquiped = false;

        CurrentWeapon.gameObject.SetActive(false);
    }

    public void EquipCurrentWeapon()
    {
        CurrentWeapon.gameObject.SetActive(true);

        toShowHud = true;
        IsWeaponEquiped = true;
    }

    private void SetWeapon(Weapon weapon)
    {
        CurrentWeapon = weapon;
        IsWeaponEquiped = true;
    }

    public void FixedUpdate()
    {
        if (health.CurrentHealth <= 0)
            return;

        if (IsReloading)
        {
            HandleReload();
        }
        else
        {
            if (enemiesCollider.CurrentTarget == null || enemiesCollider.TargetHealth == null)
            {
                //if (ArmIK)
                //    ArmIK.enabled = false;

                anim.SetBool(aimAnimBool, false);

                if (toShowHud)
                {
                    currentGap = Mathf.Lerp(currentGap, crosshairGapNotOnTarget, crosshairGapSpeed);
                    targetCrosshair.SetGap((int)currentGap, true);
                }
            }
            else
            {
                if (CanCharacterShoot())
                    SetArmIK(true);
                if (skillManager != null && skillManager.IsActivatingSkill)
                    SetArmIK(false);

                anim.SetBool(aimAnimBool, true);

                if (toShowHud)
                {
                    currentGap = Mathf.Lerp(currentGap, crosshairGapOnTarget, crosshairGapSpeed);
                    targetCrosshair.SetGap((int)currentGap, true);
                }

                if (IsWeaponEquiped)
                {
                    if (canReload && CurrentWeapon.IsMagazineEmpty())
                    {
                        StartReload();
                    }
                    else if (CurrentWeapon.CanShoot() && CanCharacterShoot())
                    {
                        CurrentWeapon.StartShoot(enemiesCollider.TargetHealth.CenterOfMass.position);

                        if (CurrentWeapon.OverrideAnimatorParam)
                            anim.SetTrigger(CurrentWeapon.AnimatorParam);
                        else
                            anim.SetTrigger(shotAnimTrigger);

                        UpdateWeaponUI();
                    }
                }
                /*else if (superSkillManager.IsSkillActive)
                {
                    superSkillManager.CurrentSkill.rangeFromTarget
                }*/
            }
        }
    }

    private bool CanCharacterShoot()
    {
        if (moveBehaviour != null && (moveBehaviour.IsRunning || moveBehaviour.IsRolling || moveBehaviour.IsDancing))
            return false;

        if (throwManager != null && throwManager.IsAimingThrow)
            return false;

        if (skillManager != null && skillManager.IsActivatingSkill)
            return false;

        return true;
    }

    public void SetArmIK(bool value)
    {
        if (ArmIK)
            ArmIK.enabled = value;
    }

    public void StartReload()
    {
        reloadStart = Time.time;
        reloadStatus = 0;

        if (toShowHud)
            imageReload.fillAmount = reloadStatus;

        SetArmIK(false);

        anim.SetBool(reloadAnimBool, true);

        IsReloading = true;

        UpdateAimUI();
    }

    public void StopReload()
    {
        anim.SetBool(reloadAnimBool, false);

        IsReloading = false;

        UpdateAimUI();
    }

    private void HandleReload()
    {
        reloadStatus = (Time.time - reloadStart) / CurrentWeapon.ReloadDuration;

        UpdateReloadUI();

        if (reloadStatus >= 1)
        {
            EndReload();
        }
    }

    private void EndReload()
    {
        CurrentWeapon.Reload();

        anim.SetBool(reloadAnimBool, false);

        IsReloading = false;

        UpdateWeaponUI();
        UpdateAimUI();
    }

    public void UpdateReloadUI()
    {
        if (toShowHud)
            imageReload.fillAmount = reloadStatus;
    }

    public void UpdateWeaponUI()
    {
        if (toShowHud)
            txtAmmo.text = CurrentWeapon.GetAmmoString();
    }

    public void UpdateAimUI()
    {
        if (toShowHud)
        {
            imageReload.gameObject.SetActive(IsReloading);
            objTarget.SetActive(!IsReloading && CurrentWeapon.CanShoot());
        }
    }
}
