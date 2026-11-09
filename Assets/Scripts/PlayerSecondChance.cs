
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerSecondChance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LuckyNumberManager luckyNumberManager;

    [Header("Revive Settings")]
    [SerializeField] private int reviveHealth = 7;

    [Header("Game Over")]
    [SerializeField] private Behaviour movementToDisable;

    private PlayerHealth playerHealth;

    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    void OnEnable()
    {
        playerHealth.HealthDepleted += HandlePlayerDeath;
    }

    void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthDepleted -= HandlePlayerDeath;
        }
    }

    void HandlePlayerDeath()
    {
        Debug.Log("Player died! Rolling for a second chance...");

        if (luckyNumberManager != null &&
            luckyNumberManager.TrySecondChance())
        {
            playerHealth.Revive(reviveHealth);
            Debug.Log("SECOND LIFE ACTIVATED!");
        }
        else
        {
            Debug.Log("GAME OVER!");

            if (movementToDisable != null)
            {
                movementToDisable.enabled = false;
            }
        }
    }
}
