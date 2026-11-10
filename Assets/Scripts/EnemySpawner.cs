
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject enemy;
    [SerializeField] private Collider startingPlatform;

    [Header("Spawn Settings")]
    [SerializeField] private float triggerHeight = 15f;
    [SerializeField] private float platformClearance = 0.15f;

    private float startingPlayerY;
    private bool hasSpawned;

    void Start()
    {
        if (player == null || enemy == null || startingPlatform == null)
        {
            Debug.LogError("Enemy Spawner is missing a reference!");
            enabled = false;
            return;
        }

        startingPlayerY = player.position.y;

        // Keep the enemy hidden until the player climbs high enough.
        enemy.SetActive(false);
    }

    void Update()
    {
        if (hasSpawned)
            return;

        if (player.position.y >= startingPlayerY + triggerHeight)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        CharacterController enemyController =
            enemy.GetComponent<CharacterController>();

        // Position the enemy's feet just above the starting platform.
        float spawnY = startingPlatform.bounds.max.y + platformClearance;

        if (enemyController != null)
        {
            spawnY +=
                enemyController.height * 0.5f -
                enemyController.center.y;
        }

        enemy.transform.position = new Vector3(
            startingPlatform.bounds.center.x,
            spawnY,
            player.position.z
        );

        enemy.SetActive(true);
        hasSpawned = true;

        Debug.Log("DICE SPIDER HAS SPAWNED!");

        enabled = false;
    }
}
