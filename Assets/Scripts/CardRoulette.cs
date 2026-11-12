using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class CardRoulette : MonoBehaviour
{
    // All of the cards in roulette roster
    public GameObject card;

    // Speed when moving
    public float moveSpeed = 1000f;

    private bool canPressSpace = false;

    // Starting position of cards
    private Vector3 startPos;

    // Sets position at which the roulette repeats
    private float repeatPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Sets repeating point
        repeatPos = GetComponent<BoxCollider>().size.x;

        StartCoroutine(WaitToPressSpace());
    }

    // Update is called once per frame
    void Update()
    {
        // Moves cards left
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // Move card1 back to start once it reaches the repeating point
        if (card.transform.position.x < startPos.x - repeatPos)
        {
            card.transform.position = new Vector3(1992, 450, 0);
        }

        if(canPressSpace && Input.GetKeyDown(KeyCode.Space))
        {
            moveSpeed = 0;
        }
    }

    IEnumerator WaitToPressSpace()
    {
        Debug.Log("Please wait 3 seconds to press the spacebar.");
        yield return new WaitForSeconds(3);
        Debug.Log("Press the spacebar now!");
        canPressSpace = true;
    }
}
