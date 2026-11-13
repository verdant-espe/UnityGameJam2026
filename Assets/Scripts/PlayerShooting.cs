
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Shooting Settings")]
    [SerializeField] public float fireRate = 0.2f;

    
[Header("Projectile Upgrades")]
public int projectileDamage = 1;
public float projectileKnockback = 8f;


    private float nextFireTime;

    private void Update()
    {
        // Hold left mouse button to fire.
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            return;
        }

        // The arm's local right direction points toward the cursor.
        Vector3 shootDirection = firePoint.right;

        // Spawn the projectile at the fingertips.
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        // Give the projectile its travel direction.
        PlayerProjectile projectileScript =
            projectile.GetComponent<PlayerProjectile>();

        if (projectileScript != null)
        {
            projectileScript.Initialize(shootDirection);

            projectileScript.ConfigureCombat(
                projectileDamage,
                projectileKnockback
            );
        }

        // Play a randomized gunshot when a projectile is fired.
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayRabbitGunshot();
        }
    }
}
