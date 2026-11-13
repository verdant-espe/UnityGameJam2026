
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyContactDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float hitCooldown = 2.5f;

    [Header("Contact Detection")]
    [SerializeField] private float contactPadding = 0.2f;


    [Header("Player Knockback")]
    [SerializeField] private float horizontalStrength = 6f;
    [SerializeField] private float verticalStrength = 4f;
    [SerializeField] private float stunDuration = 0.15f;


    private CharacterController controller;
    private float nextAllowedHitTime;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {

        if (Time.timeScale == 0f)
        {
            return;
        }

        if (Time.time < nextAllowedHitTime)
            return;

        // Calculate the spider's collision capsule.

        // Use the spider's large trigger Box Collider as its damage area.
        BoxCollider hitbox = GetComponent<BoxCollider>();

        if (hitbox == null || !hitbox.enabled || !hitbox.isTrigger)
            return;

        Vector3 center = hitbox.transform.TransformPoint(hitbox.center);

        Vector3 scale = hitbox.transform.lossyScale;

        Vector3 halfExtents = Vector3.Scale(
            hitbox.size * 0.5f,
            new Vector3(
                Mathf.Abs(scale.x),
                Mathf.Abs(scale.y),
                Mathf.Abs(scale.z)
            )
        );

        halfExtents += Vector3.one * contactPadding;

        Collider[] contacts = Physics.OverlapBox(
            center,
            halfExtents,
            hitbox.transform.rotation,
            ~0,
            QueryTriggerInteraction.Ignore
        );

               foreach (Collider contact in contacts)
        {
            PlayerHealth playerHealth =
                contact.GetComponentInParent<PlayerHealth>();

            if (playerHealth == null || playerHealth.IsDead)
                continue;

            int previousHealth = playerHealth.CurrentHealth;

            playerHealth.TakeDamage(damage);

            if (playerHealth.CurrentHealth < previousHealth)
            {
                nextAllowedHitTime =
                    Time.time + hitCooldown;


                PlayerMovement playerMovement =
                    playerHealth.GetComponent<PlayerMovement>();

                if (playerMovement != null)
                {
                    float direction = Mathf.Sign(
                        playerMovement.transform.position.x -
                        transform.position.x
                    );

                    if (direction == 0f)
                        direction = 1f;

                    playerMovement.ApplyHitKnockback(
                        direction,
                        horizontalStrength,
                        verticalStrength,
                        stunDuration
                    );
                }



                EnemyChase enemyChase = GetComponent<EnemyChase>();

                if (enemyChase != null && enemyChase.enabled)
                {
                    enemyChase.BeginHitRecovery();
                }

                // Flying enemies use their own movement controller.
                GenieFloatChase genieChase =
                    GetComponent<GenieFloatChase>();

                if (genieChase != null && genieChase.enabled)
                {
                    genieChase.BeginHitRecovery();
                }


                Debug.Log(
                    "SPIDER HIT PLAYER! Health: " +
                    playerHealth.CurrentHealth + "/" +
                    playerHealth.MaxHealth
                );
            }

            break;
        }
    }
}
