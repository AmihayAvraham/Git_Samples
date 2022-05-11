using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private GameObject parentCharacter;

    [Header("Bullets")]
    [SerializeField]
    private bool usingBullets = true;
    [SerializeField]
    private int bulletsPerShoot = 1;
    [SerializeField]
    private bool randomSpread = false;
    [SerializeField]
    private Vector3 spreadRange;
    [SerializeField]
    private float timeBetweenShoots;
    [SerializeField]
    private Transform posProjectileSpawn;
    [SerializeField]
    private List<Transform> ProjectileSpawnPositions;
    [SerializeField]
    private Transform parentFlash;
    [SerializeField]
    private List<Transform> ParentsFlash;

    [Header("Magazine")]
    [SerializeField]
    private float durationReload;
    [SerializeField]
    private int magazineCapacity;

    [SerializeField]
    private bool overrideAnimatorParam = false;
    [SerializeField]
    private string animatorParam;

    [SerializeField]
    private bool toPoolBullet = true;
    [SerializeField]
    private GameObject prefabBullet;
    [SerializeField]
    private MMObjectPooler bulletObjectPooler;

    [SerializeField]
    private GameObject prefabFlash;

    [SerializeField]
    private MMFeedbacks feedbackOnShoot;

    public int BulletsRemained { get; set; }

    private float lastShootTime = -1;
    private int projectilePosIndex = 0;

    public bool UsingBullets 
    { 
        get { return usingBullets; } 
    }

    public bool OverrideAnimatorParam
    {
        get { return overrideAnimatorParam; }
    }

    public string AnimatorParam
    {
        get { return animatorParam; }
    }

    public float ReloadDuration
    {
        get { return durationReload; }
    }

    // Start is called before the first frame update
    void Start()
    {
        Reload();
    }

    public string GetAmmoString()
    {
        return string.Format("{0}/{1}", BulletsRemained, magazineCapacity);
    }

    public bool IsMagazineEmpty()
    {
        return BulletsRemained <= 0;
    }

    public bool CanShoot()
    {
        return !IsMagazineEmpty() && (Time.time - lastShootTime >= timeBetweenShoots);
    }

    GameObject nextBullet;
    private Vector3 posProjectile;
    private Transform currentFlashParent;
    public void StartShoot(Vector3 target)
    {
        for (int i = 0; i < bulletsPerShoot; i++)
        {
            Shoot(target);
        }

        Instantiate(prefabFlash, currentFlashParent);

        if (feedbackOnShoot)
            feedbackOnShoot.PlayFeedbacks();
    }

    private Vector3 randomSpreadDirection;
    private Vector3 dirAim;
    private GameObject Shoot(Vector3 target)
    {
        if (BulletsRemained <= 0)
            return null;

        lastShootTime = Time.time;

        if (randomSpread)
        {
            randomSpreadDirection.x = Random.Range(-spreadRange.x, spreadRange.x);
            randomSpreadDirection.y = Random.Range(-spreadRange.y, spreadRange.y);
            randomSpreadDirection.z = Random.Range(-spreadRange.z, spreadRange.z);
        }
        else
        {
            randomSpreadDirection = Vector3.zero;
        }

        if (toPoolBullet)
            nextBullet = bulletObjectPooler.GetPooledGameObject();
        else
            nextBullet = Instantiate(prefabBullet, posProjectileSpawn);

        if (nextBullet == null) { return null; }
        if (ProjectileSpawnPositions != null && ProjectileSpawnPositions.Count > 0)
        {
            posProjectile = ProjectileSpawnPositions[projectilePosIndex].position;
            currentFlashParent = ParentsFlash[projectilePosIndex];

            projectilePosIndex++;
            if (projectilePosIndex >= ProjectileSpawnPositions.Count)
                projectilePosIndex = 0;
        }
        else
        {
            posProjectile = posProjectileSpawn.position;
            currentFlashParent = parentFlash;
        }

        nextBullet.transform.position = posProjectile;
        nextBullet.gameObject.SetActive(true);

        Projectile projectile = nextBullet.GetComponent<Projectile>();
        /*if (projectile != null)
        {
            projectile.SetWeapon(this);
            if (Owner != null)
            {
                projectile.SetOwner(Owner.gameObject);
            }
        }*/

        if (projectile != null)
        {
            if (parentCharacter != null)
                projectile.SetOwner(parentCharacter);

            dirAim = (target + randomSpreadDirection) - posProjectile;
            projectile.SetDirection(dirAim, Quaternion.LookRotation(dirAim), true);
        }

        BulletsRemained--;

        return (nextBullet);
    }

    public void Reload()
    {
        BulletsRemained = magazineCapacity;
    }
}
