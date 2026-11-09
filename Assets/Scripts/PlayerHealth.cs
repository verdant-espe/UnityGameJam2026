
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 7;

    public int CurrentHealth { get; private set; }
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

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

        HealthChanged?.Invoke(CurrentHealth, MaxHealth);

        Debug.Log("Player Health: " + CurrentHealth + "/" + MaxHealth);

        if (CurrentHealth == 0)
        {
            IsDead = true;

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
