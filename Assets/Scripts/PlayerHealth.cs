
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] public int maxHealth = 7;
    [Header("Tarot Effects")]
    public int damageTakenMultiplier = 1;

    public int CurrentHealth { get; set; }
    public int MaxHealth => maxHealth;
    public bool IsDead { get; private set; }

    // Other scripts can listen for health changes and death.
    public event Action<int, int> HealthChanged;
    public event Action HealthDepleted;

    void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        CurrentHealth = maxHealth;
        IsDead = false;
    }

    // Called when an enemy damages the player.
    public void TakeDamage(int damage)
    {

        if (IsDead || damage <= 0)
            return;

        // Apply any tarot damage penalty.
        damage *= damageTakenMultiplier;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);


        // Play a random rabbit damage vocalization.
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayRabbitDamage();
        }

        HealthChanged?.Invoke(CurrentHealth, MaxHealth);

        Debug.Log("Player Health: " + CurrentHealth + "/" + MaxHealth);

        if (CurrentHealth == 0)
        {
            IsDead = true;

            // Play the rabbit's death vocalization.
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayRabbitDeath();
            }

            Debug.Log("Health reached zero! Awaiting lucky-number result.");

            HealthDepleted?.Invoke();
        }
    }

    // Restore health without exceeding maximum health.
    public void Heal(int amount)
    {
        if (IsDead || amount <= 0)
            return;

        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);

        HealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    // Called later if the player wins their lucky-number roll.
    public void Revive(int restoredHealth)
    {
        if (!IsDead)
            return;

        IsDead = false;

        CurrentHealth = Mathf.Clamp(restoredHealth, 1, MaxHealth);

        HealthChanged?.Invoke(CurrentHealth, MaxHealth);

        Debug.Log("Lucky revive! Health restored to " + CurrentHealth);
    }

    // Temporary testing option in the Unity Inspector.
    [ContextMenu("TEST: Take 1 Damage")]
    private void TestDamage()
    {
        if (Application.isPlaying)
            TakeDamage(1);
    }
}
