using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CardRoulette : MonoBehaviour
{
    // All of the cards in roulette roster
    public GameObject card;

    // Speed when moving
    private float moveSpeed = 1000f;

    // Starting position of cards
    private Vector3 startPos;

    // Start position of roulette
    private Vector3 cardPos;

    // Sets position at which the roulette repeats
    private float repeatPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Tracks the position of each card
        cardPos = GameObject.FindGameObjectWithTag("Card").transform.position;

        // Sets repeating point
        repeatPos = GetComponent<BoxCollider>().size.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Moves cards left
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // Gradually reduces the speed
        moveSpeed = 0.9995f * moveSpeed;

        // Move card1 back to start once it reaches the repeating point
        if (card.transform.position.x < startPos.x - repeatPos)
        {
            card.transform.position = new Vector3(1992, 450, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If a card enters the trigger of the pointer once roulette stops, that card is selected
        if (other.gameObject.CompareTag("Pointer"))
        {
            
        }
    }
}
