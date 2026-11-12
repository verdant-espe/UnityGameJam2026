
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerSecondChance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LuckyNumberManager luckyNumberManager;

    [Header("D100 Second Chance Display")]
    [SerializeField] private GameObject luckyRollPanel;
    [SerializeField] private D100RollAnimation diceAnimation;
    [SerializeField] private TMP_Text diceResultText;
    [SerializeField] private Camera diceCamera;
    [SerializeField] private float resultDisplayDuration = 2f;

    [Header("Revive Settings")]
    [SerializeField] private int reviveHealth = 7;

    [Header("Game Over")]
    [SerializeField] private Behaviour movementToDisable;

    private PlayerHealth playerHealth;
    private bool isResolvingDeath;
    private bool pausedBySecondChance;

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

        if (pausedBySecondChance)
        {
            Time.timeScale = 1f;
            pausedBySecondChance = false;
        }
    }

    void HandlePlayerDeath()
    {
        if (isResolvingDeath)
            return;

        // The player only receives one second-chance roll.
        if (luckyNumberManager == null ||
            !luckyNumberManager.HasLuckyNumber ||
            luckyNumberManager.LastRolledNumber != -1)
        {
            ShowGameOver("GAME OVER");
            return;
        }

        StartCoroutine(SecondChanceRoutine());
    }

    private IEnumerator SecondChanceRoutine()
    {
        isResolvingDeath = true;

        // Pause the rabbit, spider, and world.
        Time.timeScale = 0f;
        pausedBySecondChance = true;

        if (luckyRollPanel != null)
            luckyRollPanel.SetActive(true);

        if (diceResultText != null)
            diceResultText.text = "Rolling for your second chance...";

        if (diceCamera != null)
        {
            diceCamera.gameObject.SetActive(true);
            diceCamera.enabled = true;
        }

        if (diceAnimation != null)
            diceAnimation.gameObject.SetActive(true);

        // Roll once and save the actual result.
        bool survived = luckyNumberManager.TrySecondChance();

        // Wait for the dice animation to finish.
        if (diceAnimation != null)
        {
            bool rollFinished = false;

            diceAnimation.Roll(() => rollFinished = true);

            yield return new WaitUntil(() => rollFinished);
        }

        int rolledNumber = luckyNumberManager.LastRolledNumber;

        if (diceResultText != null)
        {
            diceResultText.text =
                "D100 Result: " + rolledNumber +
                (survived ? "\nLUCKY MATCH!" : "\nNO MATCH!");
        }

        // Give the player time to see their result.
        yield return new WaitForSecondsRealtime(
            Mathf.Max(0f, resultDisplayDuration)
        );

        if (survived)
        {
            playerHealth.Revive(reviveHealth);

            Debug.Log("SECOND LIFE ACTIVATED!");

            if (luckyRollPanel != null)
                luckyRollPanel.SetActive(false);

            if (diceCamera != null)
                diceCamera.gameObject.SetActive(false);

            Time.timeScale = 1f;
            pausedBySecondChance = false;
            isResolvingDeath = false;
        }
        else
        {
            ShowGameOver(
                "D100 Result: " + rolledNumber +
                "\nGAME OVER"
            );
        }
    }

    private void ShowGameOver(string message)
    {
        if (luckyRollPanel != null)
            luckyRollPanel.SetActive(true);

        if (diceResultText != null)
            diceResultText.text = message;

        if (diceCamera != null)
            diceCamera.gameObject.SetActive(false);

        if (movementToDisable != null)
            movementToDisable.enabled = false;

        Time.timeScale = 0f;
        pausedBySecondChance = true;

        Debug.Log("GAME OVER!");
    }
}
