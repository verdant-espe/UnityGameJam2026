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
        // Remember where this card starts.
        startPos = transform.position;

        // UI cards use RectTransform, not BoxCollider.
        RectTransform cardRect = GetComponent<RectTransform>();

        repeatPos = cardRect.rect.width * transform.lossyScale.x;
               }


    private void OnEnable()
    {
        // Reset the roulette every time it opens.
        moveSpeed = 1000f;

        // Prevent Space from stopping it immediately.
        canPressSpace = false;

        // Start the countdown for this spin.
        StartCoroutine(WaitToPressSpace());
    }



    // Update is called once per frame
    void Update()
    {

        transform.Translate(Vector3.left * moveSpeed * Time.unscaledDeltaTime);

        // Move card1 back to start once it reaches the repeating point

        RectTransform rouletteArea =
            transform.parent.GetComponent<RectTransform>();

        float leftEdge = rouletteArea.TransformPoint(
            new Vector3(rouletteArea.rect.xMin, 0f, 0f)
        ).x;

        // When a card completely leaves the left side...
        if (card.transform.position.x + repeatPos * 0.5f < leftEdge)
        {
            float rightmostX = float.NegativeInfinity;

            // Find the rightmost card in the roulette.
            foreach (Transform sibling in transform.parent)
            {
                if (sibling.GetComponent<CardRoulette>() != null)
                {
                    rightmostX = Mathf.Max(
                        rightmostX,
                        sibling.position.x
                    );
                }
            }

            // Place this card immediately after the last card.
            card.transform.position = new Vector3(
                    rightmostX + repeatPos * 2f,
                card.transform.position.y,
                card.transform.position.z
            );
        }

        if(canPressSpace && Input.GetKeyDown(KeyCode.Space))
        {
            moveSpeed = 0;
        }
    }

    IEnumerator WaitToPressSpace()
    {
        Debug.Log("Please wait 3 seconds to press the spacebar.");
        yield return new WaitForSecondsRealtime(3);
        Debug.Log("Press the spacebar now!");
        canPressSpace = true;
    }
}
