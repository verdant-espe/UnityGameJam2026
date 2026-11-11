
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
        if (Time.time < nextAllowedHitTime)
            return;

        // Calculate the spider's collision capsule.
        Vector3 center =
            transform.TransformPoint(controller.center);

        Vector3 scale = transform.lossyScale;

        float radius = controller.radius *
            Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));

        float height =
            controller.height * Mathf.Abs(scale.y);

        float halfSegment =
            Mathf.Max(0f, height * 0.5f - radius);

        Vector3 bottom =
            center - Vector3.up * halfSegment;

        Vector3 top =
            center + Vector3.up * halfSegment;

        // Look for colliders touching or very close
        // to the spider's body.
        Collider[] contacts = Physics.OverlapCapsule(
            bottom,
            top,
            radius + contactPadding,
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

                if (enemyChase != null)
                {
                    enemyChase.BeginHitRecovery();
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
