
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject monkey;
    [SerializeField] private GameObject genie;
    [SerializeField] private Collider startingPlatform;

    [Header("Spawn Settings")]
    [SerializeField] private float triggerHeight = 15f;
    [SerializeField] private float platformClearance = 0.15f;

    [Header("Random Enemy Arrival")]
    [SerializeField] private float minGenieDelay = 10f;
    [SerializeField] private float maxGenieDelay = 25f;

    [Header("Genie Spawn Position")]
    [SerializeField] private float genieHorizontalDistance = 18f;
    [SerializeField] private float genieVerticalOffset = 5f;

    private float startingPlayerY;
    private bool hasSpawned;
    private bool waitingForNextEnemy;

    // 1 = Monkey, 2 = Genie, 3 = Random encounters.
    private int nextStage = 1;

    private GameObject activeEnemy;

    private EnemyHealth spiderHealth;
    private EnemyHealth monkeyHealth;
    private EnemyHealth genieHealth;

    private void Start()
    {
        if (player == null || enemy == null ||
            monkey == null || genie == null ||
            startingPlatform == null)
        {
            Debug.LogError("EnemySpawner is missing a reference!");
            enabled = false;
            return;
        }

        startingPlayerY = player.position.y;

        spiderHealth = enemy.GetComponent<EnemyHealth>();
        monkeyHealth = monkey.GetComponent<EnemyHealth>();
        genieHealth = genie.GetComponent<EnemyHealth>();

        if (spiderHealth == null ||
            monkeyHealth == null ||
            genieHealth == null)
        {
            Debug.LogError("An enemy is missing EnemyHealth!");
            enabled = false;
            return;
        }

        spiderHealth.Defeated += OnEnemyDefeated;
        monkeyHealth.Defeated += OnEnemyDefeated;
        genieHealth.Defeated += OnEnemyDefeated;

        enemy.SetActive(false);
        monkey.SetActive(false);
        genie.SetActive(false);
    }

    private void Update()
    {
        if (hasSpawned)
            return;

        if (player.position.y >= startingPlayerY + triggerHeight)
        {
            hasSpawned = true;

            PlaceOnPlatform(enemy, startingPlatform);
            ActivateEnemy(enemy);

            Debug.Log("DICE SPIDER HAS SPAWNED!");
        }
    }

    private void OnEnemyDefeated(EnemyHealth defeatedEnemy)
    {
        if (defeatedEnemy.gameObject != activeEnemy ||
            waitingForNextEnemy)
            return;

        Debug.Log(defeatedEnemy.name + " defeated!");

        activeEnemy = null;
        StartCoroutine(SpawnNextEnemy());
    }

    private IEnumerator SpawnNextEnemy()
    {
        waitingForNextEnemy = true;

        float minimum = Mathf.Max(0f, minGenieDelay);
        float maximum = Mathf.Max(minimum, maxGenieDelay);

        float delay = Random.Range(minimum, maximum);

        Debug.Log("Next enemy arrives in " + delay + " seconds.");

        yield return new WaitForSeconds(delay);

        GameObject nextEnemy;

        if (nextStage == 1)
        {
            nextEnemy = monkey;
            nextStage = 2;
        }
        else if (nextStage == 2)
        {
            nextEnemy = genie;
            nextStage = 3;
        }
        else
        {
            // Endless mode: choose one of our three enemies.
            int randomEnemy = Random.Range(0, 3);

            if (randomEnemy == 0)
                nextEnemy = enemy;
            else if (randomEnemy == 1)
                nextEnemy = monkey;
            else
                nextEnemy = genie;
        }

        if (nextEnemy == genie)
        {
            float side = Random.value < 0.5f ? -1f : 1f;

            genie.transform.position = player.position +
                new Vector3(
                    side * genieHorizontalDistance,
                    genieVerticalOffset,
                    0f
                );
        }
        else
        {
            // Find a nearby platform for ground-based enemies.
            Collider platform = FindNearbyPlatform();

            // Wait until a suitable platform is available.
            while (platform == null)
            {
                yield return new WaitForSeconds(1f);
                platform = FindNearbyPlatform();
            }

            PlaceOnPlatform(nextEnemy, platform);
        }

        ActivateEnemy(nextEnemy);

        Debug.Log("NEW ENEMY SPAWNED: " + nextEnemy.name);

        waitingForNextEnemy = false;
    }

    private Collider FindNearbyPlatform()
    {
        Collider[] platforms = Physics.OverlapSphere(
            player.position,
            25f,
            LayerMask.GetMask("Ground"),
            QueryTriggerInteraction.Ignore
        );

        Collider bestPlatform = null;
        float bestScore = float.MaxValue;

        foreach (Collider platform in platforms)
        {
            float horizontalDistance = Mathf.Abs(
                platform.bounds.center.x - player.position.x
            );

            float verticalDistance = Mathf.Abs(
                platform.bounds.max.y - player.position.y
            );

            // Avoid spawning directly on the rabbit
            // or too far above/below it.
            if (horizontalDistance < 5f ||
                horizontalDistance > 22f ||
                verticalDistance > 12f)
                continue;

            float score = verticalDistance +
                Mathf.Abs(horizontalDistance - 12f) * 0.35f;

            if (score < bestScore)
            {
                bestScore = score;
                bestPlatform = platform;
            }
        }

        return bestPlatform;
    }

    private void PlaceOnPlatform(GameObject target, Collider platform)
    {
        CharacterController controller =
            target.GetComponent<CharacterController>();

        float spawnY = platform.bounds.max.y + platformClearance;

        if (controller != null)
        {
            spawnY += controller.height * 0.5f -
                controller.center.y;
        }

        target.transform.position = new Vector3(
            platform.bounds.center.x,
            spawnY,
            player.position.z
        );
    }

    private void ActivateEnemy(GameObject target)
    {
        activeEnemy = target;
        target.SetActive(true);
    }

    private void OnDestroy()
    {
        if (spiderHealth != null)
            spiderHealth.Defeated -= OnEnemyDefeated;

        if (monkeyHealth != null)
            monkeyHealth.Defeated -= OnEnemyDefeated;

        if (genieHealth != null)
            genieHealth.Defeated -= OnEnemyDefeated;
    }
}
