
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    [SerializeField] private int maxHealth = 7;

    public int CurrentHealth { get; private set; }

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    private bool isDead = false;

    void OnEnable()
    {
        // Start with full health whenever this
        // enemy is activated or respawned.
        CurrentHealth = Mathf.Max(1, maxHealth);
        isDead = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        CurrentHealth = Mathf.Max(
            0,
            CurrentHealth - damage
        );

        Debug.Log(
            gameObject.name + " took " + damage +
            " damage! Health: " +
            CurrentHealth + "/" + MaxHealth
        );

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log(gameObject.name + " defeated!");

        // Remove the defeated enemy from the scene.
        Destroy(gameObject);
    }

    // Temporary testing option in the Inspector.
    [ContextMenu("Test Take 1 Damage")]
    private void TestDamage()
    {
        if (Application.isPlaying)
        {
            TakeDamage(1);
        }
    }
}
