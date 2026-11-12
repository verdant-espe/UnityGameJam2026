
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private Transform player;

    [Header("Tarot Platform Settings")]
    [SerializeField, Range(0f, 1f)]
    private float tarotPlatformChance = 0.20f;

    [Header("Generation Settings")]
    [SerializeField] private int startingPlatforms = 15;
    [SerializeField] private float generateAheadDistance = 25f;

    [Header("Level Boundaries")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    [Header("Difficulty Progression")]
    [SerializeField] private float fullDifficultyHeight = 1200f;

    [Header("Normal Vertical Spacing")]
    [SerializeField]
    private Vector2 easyVerticalGap =
        new Vector2(3f, 4.5f);

    [SerializeField]
    private Vector2 hardVerticalGap =
        new Vector2(4.5f, 6.5f);

    [Header("Horizontal Spacing")]
    [SerializeField] private float easyHorizontalStep = 3f;
    [SerializeField] private float hardHorizontalStep = 6f;

    [Header("Platform Width")]
    [SerializeField] private float easyWidthMultiplier = 1.45f;
    [SerializeField] private float hardWidthMultiplier = 0.70f;

    [SerializeField] private float easyWidthVariation = 0.15f;
    [SerializeField] private float hardWidthVariation = 0.25f;

    [Header("Double Jump Challenges")]
    [SerializeField] private float doubleJumpStartHeight = 350f;
    [SerializeField] private float doubleJumpFullChanceHeight = 900f;

    [Range(0f, 1f)]
    [SerializeField] private float maxDoubleJumpChance = 0.30f;

    [SerializeField]
    private Vector2 doubleJumpVerticalGap =
        new Vector2(7.8f, 9.5f);

    [SerializeField] private float doubleJumpHorizontalStep = 3f;

    private float previousX = 0f;
    private float nextY = 0f;

    void Start()
    {
        if (platformPrefab == null)
        {
            Debug.LogError("Platform Prefab is missing!");
            enabled = false;
            return;
        }

        for (int i = 0; i < startingPlatforms; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        if (player == null)
            return;

        while (nextY < player.position.y + generateAheadDistance)
        {
            SpawnPlatform();
        }
    }

    void SpawnPlatform()
    {
        // Calculate difficulty from current height.
        float difficulty = Mathf.Clamp01(
            nextY / Mathf.Max(1f, fullDifficultyHeight)
        );

        // Normal vertical gap range.
        float minGap = Mathf.Lerp(
            easyVerticalGap.x,
            hardVerticalGap.x,
            difficulty
        );

        float maxGap = Mathf.Lerp(
            easyVerticalGap.y,
            hardVerticalGap.y,
            difficulty
        );

        // Increase the chance of double-jump challenges.
        float challengeProgress = Mathf.InverseLerp(
            doubleJumpStartHeight,
            doubleJumpFullChanceHeight,
            nextY
        );

        float doubleJumpChance =
            challengeProgress * maxDoubleJumpChance;

        bool doubleJumpChallenge =
            nextY >= doubleJumpStartHeight &&
            Random.value < doubleJumpChance;

        float verticalGap;
        float horizontalStep;

        if (doubleJumpChallenge)
        {
            // High jump: requires a well-timed double jump.
            verticalGap = Random.Range(
                doubleJumpVerticalGap.x,
                doubleJumpVerticalGap.y
            );

            // Reduce sideways travel on very high jumps.
            horizontalStep = doubleJumpHorizontalStep;
        }
        else
        {
            // Normal jump with progressive difficulty.
            verticalGap = Random.Range(minGap, maxGap);

            horizontalStep = Mathf.Lerp(
                easyHorizontalStep,
                hardHorizontalStep,
                difficulty
            );

            // Higher normal jumps have less sideways distance.
            float gapDifficulty = Mathf.InverseLerp(
                minGap, maxGap, verticalGap
            );

            horizontalStep *= Mathf.Lerp(
                1f, 0.85f, gapDifficulty
            );
        }

        // Move upward.
        nextY += Mathf.Max(0.5f, verticalGap);

        // Move randomly left or right, within level bounds.

        // Minimum sideways distance increases with difficulty.
        float minHorizontalStep = doubleJumpChallenge
    ? Mathf.Lerp(11f, 14f, difficulty)
    : Mathf.Lerp(5.5f, 10f, difficulty);

        // Check how much space is available in each direction.
        float leftRoom = previousX - minX;
        float rightRoom = maxX - previousX;

        bool canGoLeft = leftRoom >= minHorizontalStep;
        bool canGoRight = rightRoom >= minHorizontalStep;

        // Choose a direction, avoiding the level boundaries.
        float direction;

        if (canGoLeft && canGoRight)
        {
            direction = Random.value < 0.5f ? -1f : 1f;
        }
        else if (canGoLeft)
        {
            direction = -1f;
        }
        else if (canGoRight)
        {
            direction = 1f;
        }
        else
        {
            direction = rightRoom > leftRoom ? 1f : -1f;
        }

        // Generate an offset that cannot be almost zero.
        float availableRoom = direction < 0f
            ? leftRoom
            : rightRoom;

        float maxStep = Mathf.Min(horizontalStep, availableRoom);
        float minStep = Mathf.Min(minHorizontalStep, maxStep);

        float randomStep = Random.Range(minStep, maxStep);

        previousX = Mathf.Clamp(
            previousX + direction * randomStep,
            minX,
            maxX
        );


        // Create platform.
        Vector3 spawnPosition = new Vector3(
            previousX, nextY, 0f
        );

        GameObject newPlatform = Instantiate(
            platformPrefab,
            spawnPosition,
            Quaternion.identity
        );

        // Platform sizes become smaller AND more varied.
        float baseWidthMultiplier = Mathf.Lerp(
            easyWidthMultiplier,
            hardWidthMultiplier,
            difficulty
        );

        float widthVariation = Mathf.Lerp(
            easyWidthVariation,
            hardWidthVariation,
            difficulty
        );

        float randomWidthMultiplier = Mathf.Clamp(
            baseWidthMultiplier +
            Random.Range(-widthVariation, widthVariation),
            0.5f,
            1.6f
        );

        Vector3 scale = newPlatform.transform.localScale;
        scale.x *= randomWidthMultiplier;
        newPlatform.transform.localScale = scale;


        // Randomly turn 20% of generated platforms
        // into tarot platforms.
        if (Random.value < tarotPlatformChance)
        {
            newPlatform.AddComponent<TarotPlatform>();

            Debug.Log("TAROT PLATFORM GENERATED!");
        }

    }
}
