using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShield : MonoBehaviour
{
    [SerializeField]
    private float shieldSize;
    [SerializeField]
    private Vector3 shieldOffset;
    [SerializeField]
    private MMObjectPooler projectileCollisionObjectPooler;

    private void OnTriggerEnter(Collider other)
    {
        Projectile collideWith = other.gameObject.GetComponent<Projectile>();
        Vector3 collisionPoint = other.ClosestPointOnBounds(transform.position);

        if (collideWith != null && Vector3.Distance(transform.position + shieldOffset, collideWith.Owner.transform.position) > shieldSize)
        {
            GameObject pooledObj = projectileCollisionObjectPooler.GetPooledGameObject();
            pooledObj.transform.position = collisionPoint;
            pooledObj.SetActive(true);

            collideWith.Kill();
        }
    }

    /*private Vector3 GetCollisionPoint(Vector3 posProjectile)
    {
        posProjectile - (transform.position + shieldOffset)
        Vector3.Lerp(transform.position + shieldOffset, posProjectile, x / (A - B).Length());
    }*/

    /*private void OnCollisionEnter(Collision collision)
    {
        Projectile collideWith = collision.collider.gameObject.GetComponent<Projectile>();

        if (collideWith != null && Vector3.Distance(transform.position + shieldOffset, collideWith.Owner.transform.position) > shieldSize)
        {
            GameObject pooledObj = projectileCollisionObjectPooler.GetPooledGameObject();
            pooledObj.transform.position = collision.contacts[0].point;
            pooledObj.SetActive(true);

            collideWith.Kill();
        }
    }*/

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + shieldOffset, shieldSize);
    }
}
