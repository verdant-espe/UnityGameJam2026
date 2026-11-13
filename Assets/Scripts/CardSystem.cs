
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    [Header("Player References")]
    public PlayerMovement player;
    public PlayerShooting shooting;
    public PlayerHealth playerHealth;

    public bool hasCard;

    [Header("Active Tarot Card")]
    [SerializeField] private float cardDuration = 30f;
    [SerializeField] private float timeRemaining;

    private GameObject activeCard;
    private EnemyChase affectedEnemy;
    private float originalEnemySpeed;

    private float normalSpeed;
    private float normalSprintSpeed;
    private float normalAirControl;
    private int normalMaxJumps;

    private float normalFireRate;
    private int normalProjectileDamage;
    private float normalProjectileKnockback;

    private int normalDamageMultiplier;

    private void Start()
    {
        // Remember the rabbit's original settings.
        normalSpeed = player.speed;
        normalSprintSpeed = player.sprintSpeed;
        normalAirControl = player.airControl;
        normalMaxJumps = player.maxJumps;

        normalFireRate = shooting.fireRate;
        normalProjectileDamage = shooting.projectileDamage;
        normalProjectileKnockback = shooting.projectileKnockback;

        normalDamageMultiplier = playerHealth.damageTakenMultiplier;
    }


    private void RestoreEnemySpeed()
    {
        if (affectedEnemy != null)
        {
            // Return the monster to its original speed.
            affectedEnemy.speed = originalEnemySpeed;

            // Clear the reference.
            affectedEnemy = null;
        }
    }

    private void RestorePlayerStats()
    {
        // Restore normal movement.
        player.speed = normalSpeed;
        player.sprintSpeed = normalSprintSpeed;
        player.airControl = normalAirControl;
        player.maxJumps = normalMaxJumps;

        // Restore normal shooting.
        shooting.fireRate = normalFireRate;
        shooting.projectileDamage = normalProjectileDamage;
        shooting.projectileKnockback = normalProjectileKnockback;

        // Restore normal damage taken.
        playerHealth.damageTakenMultiplier = normalDamageMultiplier;

        Debug.Log("Tarot player stats restored!");
    }

    private void Update()
    {
        // Only count down if a tarot card is active.
        if (activeCard == null)
            return;

        timeRemaining -= Time.deltaTime;

        // When the timer reaches zero, remove the card.
        if (timeRemaining <= 0f)
        {
            RestorePlayerStats();
            RestoreEnemySpeed();

            activeCard = null;
            hasCard = false;
            timeRemaining = 0f;

            Debug.Log("TAROT CARD EXPIRED!");
        }
    }

    // Called when the roulette selects a card.
    public void ApplyCard(GameObject selectedCard)
    {
        if (selectedCard == null)
        {
            Debug.LogWarning("No tarot card selected!");
            return;
        }

        if (player == null || shooting == null || playerHealth == null)
        {
            Debug.LogWarning("CardSystem player references are missing!");
            return;
        }

        // Remove the previous card's player effects.
        if (activeCard != null)
        {
            RestorePlayerStats();

            // Remove all effects from the previous card.
            if (activeCard != null)
            {
                RestorePlayerStats();
                RestoreEnemySpeed();
            }

            // Activate the new card and restart its timer.
            activeCard = selectedCard;
            timeRemaining = cardDuration;

        }

        // Activate the new card and restart its timer.
        activeCard = selectedCard;
        timeRemaining = cardDuration;

        switch (selectedCard.name)
        {
            case "Card1": // Triple Jump
                player.maxJumps = 3;
                IncreaseEnemySpeed();
                break;

            case "Card2": // Glass Cannon
                shooting.projectileDamage = 2;
                playerHealth.damageTakenMultiplier = 2;
                break;

            case "Card3": // Rapid Fire
                shooting.fireRate = 0.1f;
                player.speed = 3f;
                break;

            case "Card4": // Heavy Hitter
                shooting.projectileKnockback = 12f;
                shooting.fireRate = 0.4f;
                break;

            case "Card5": // Regeneration
                playerHealth.Heal(playerHealth.MaxHealth);
                IncreaseEnemySpeed();
                break;

            case "Card6": // The Chariot
                player.speed = 8f;
                player.sprintSpeed = 11f;
                player.airControl = 0.35f;
                break;

            default:
                Debug.LogWarning("Unknown tarot card: " + selectedCard.name);
                return;
        }

        hasCard = true;

        Debug.Log("TAROT EFFECT APPLIED: " + selectedCard.name);
    }


    private void IncreaseEnemySpeed()
    {
        EnemyChase[] enemies =
            FindObjectsByType<EnemyChase>(
                FindObjectsSortMode.None
            );

        foreach (EnemyChase enemy in enemies)
        {
            // Only affect the currently active monster.
            if (enemy == null || !enemy.isActiveAndEnabled)
                continue;

            // Remember the monster and its original speed.
            affectedEnemy = enemy;
            originalEnemySpeed = enemy.speed;

            // Apply the unlucky effect.
            enemy.speed = 6f;

            Debug.Log("Enemy speed increased to 6!");

            break;
        }
    }

}
