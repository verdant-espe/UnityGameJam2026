using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class AddCardToPlayer : MonoBehaviour
{
    // Makes a list of all the cards in the roulette roster
    public List<GameObject> cards = new List<GameObject>();

    private CardRoulette cardRoulette;

    // References roulette pointer
    public GameObject pointer;

    // References card slot
    public GameObject currentCard;

    // Set to true on default until roulette is stopped
    public bool rouletteSpin = true;

    // Set to false on default until pointer lands on a card
    public bool landedOnCard = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Assigns cardRoulette to all cards with the CardRoulette script
        cardRoulette = GameObject.Find("Card1").GetComponent<CardRoulette>();
        cardRoulette = GameObject.Find("Card2").GetComponent<CardRoulette>();
        cardRoulette = GameObject.Find("Card3").GetComponent<CardRoulette>();
        cardRoulette = GameObject.Find("Card4").GetComponent<CardRoulette>();
        cardRoulette = GameObject.Find("Card5").GetComponent<CardRoulette>();
        cardRoulette = GameObject.Find("Card6").GetComponent<CardRoulette>();

        // Assigns pointer to pointer game object
        pointer = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // If roulette is stopped upon hitting space, rouletteSpin is set to false and landedOnCard is set to true
        if(Input.GetKey("space") && cardRoulette.moveSpeed == 0)
        {
            rouletteSpin = false;
            landedOnCard = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If card enters trigger of the pointer, that card is selected and added to the card slot
        if(pointer == other.gameObject.CompareTag("Card"))
        {
            // Loops through the list of cards and changes currentCard to the card the pointer landed on
            //foreach (var card in cards)
            //{
                
            //}
        }
    }
}
