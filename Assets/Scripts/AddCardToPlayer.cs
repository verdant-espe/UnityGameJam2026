
using UnityEngine;
using TMPro;
using System.Collections;

public class AddCardToPlayer : MonoBehaviour
{
    [Header("Roulette Cards")]
    public GameObject[] cards;

    [Header("Existing UI References")]
    public GameObject pointer;
    public GameObject currentCard;
    [SerializeField] private TMP_Text activeCardText;
    [SerializeField] private CardSystem cardSystem;

    public bool rouletteSpin = true;
    public bool landedOnCard = false;

    public GameObject SelectedCard { get; private set; }

    private bool hasSeenMovement;
    private bool hasSelectedCard;

    private void OnEnable()
    {
        rouletteSpin = true;
        landedOnCard = false;
        hasSeenMovement = false;
        hasSelectedCard = false;
        SelectedCard = null;
    }

    private void Update()
    {
        if (hasSelectedCard || cards == null || cards.Length == 0)
            return;

        bool allCardsStopped = true;

        foreach (GameObject card in cards)
        {
            if (card == null)
                return;

            CardRoulette roulette = card.GetComponent<CardRoulette>();

            if (roulette == null)
                return;

            if (roulette.moveSpeed > 0f)
            {
                hasSeenMovement = true;
                allCardsStopped = false;
            }
        }

        if (!hasSeenMovement || !allCardsStopped)
            return;

        // Select one random card once the roulette stops.
        int randomIndex = Random.Range(0, cards.Length);

        SelectedCard = cards[randomIndex];

        
        if (cardSystem != null)
        {
            cardSystem.ApplyCard(SelectedCard);
        }
        else
        {
            Debug.LogWarning("CardSystem not found on Roulette!");
        }

        // Get the name printed on the randomly selected card.
        TMP_Text cardLabel = SelectedCard.GetComponentInChildren<TMP_Text>();

        string cardName = cardLabel != null
            ? cardLabel.text
            : SelectedCard.name;

        // Display the actual winning card.
        if (activeCardText != null)
        {
            activeCardText.text = "Selected Card: " + cardName;
        }

        hasSelectedCard = true;
        rouletteSpin = false;
        landedOnCard = true;

        Debug.Log(
    gameObject.name + " [ID: " + GetInstanceID() +
    "] SELECTED: " + SelectedCard.name,
    this
);
        StartCoroutine(CloseRouletteAfterDelay());

    }


    private IEnumerator CloseRouletteAfterDelay()
    {
        // Wait two seconds, even while the game is paused.
        yield return new WaitForSecondsRealtime(2f);

        TarotRouletteOpener opener =
            FindFirstObjectByType<TarotRouletteOpener>();

        if (opener != null)
        {
            opener.CloseRoulette();
        // If card enters trigger of the pointer and roulette is stopped, that card is selected and added to the card slot
        for(int i = 0; i < cards.Length; i++)
        {
            if (pointer == other.gameObject.CompareTag("Card") && rouletteSpin == false)
            {
                Debug.Log("A card has been selected!");

                currentCard = cards[i];
            }
        }
    }



}
