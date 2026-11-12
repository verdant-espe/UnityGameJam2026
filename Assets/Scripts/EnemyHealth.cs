
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    [SerializeField] private int maxHealth = 7;

    private EnemyAudio enemyAudio;
    public int CurrentHealth { get; private set; }

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    private bool isDead = false;

    public event System.Action<EnemyHealth> Defeated;

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

        // Play a random vocal reaction to damage.
        if (CurrentHealth > 0 && enemyAudio != null)
        {
            enemyAudio.PlayDamage();
        }

        if (CurrentHealth <= 0)
        {
            Die();
                   }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log(gameObject.name + " defeated!");

        // Play the dedicated death sound.
        if (enemyAudio != null)
        {
            enemyAudio.PlayDeath();
        }

        // Remove the defeated enemy from the scene.
        Defeated?.Invoke(this);
        gameObject.SetActive(false);
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

    private void Awake()
    {
        enemyAudio = GetComponent<EnemyAudio>();
    }
}
