using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine.UI;

public class AddCardToPlayer : MonoBehaviour
{
    // Makes a list of all the cards in the roulette roster
    public GameObject[] cards;

    // References CardRoulette script
    private CardRoulette cardRoulette;

    // References CardSystem script
    private CardSystem cardSystem;

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
        cards = GameObject.FindGameObjectsWithTag("Card");

        // Assigns cardRoulette to all cards with the CardRoulette script
        cardRoulette = GameObject.Find("Card1").GetComponent<CardRoulette>();
        cardRoulette = GameObject.Find("Card2").GetComponent<CardRoulette>();
        cardRoulette = GameObject.Find("Card3").GetComponent<CardRoulette>();
        cardRoulette = GameObject.Find("Card4").GetComponent<CardRoulette>();
        cardRoulette = GameObject.Find("Card5").GetComponent<CardRoulette>();
        cardRoulette = GameObject.Find("Card6").GetComponent<CardRoulette>();

        // Assigns cardSystem to all cards with the CardSystem script
        cardSystem = GameObject.Find("Card1").GetComponent<CardSystem>();
        cardSystem = GameObject.Find("Card2").GetComponent<CardSystem>();
        cardSystem = GameObject.Find("Card3").GetComponent<CardSystem>();
        cardSystem = GameObject.Find("Card4").GetComponent<CardSystem>();
        cardSystem = GameObject.Find("Card5").GetComponent<CardSystem>();
        cardSystem = GameObject.Find("Card6").GetComponent<CardSystem>();

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
