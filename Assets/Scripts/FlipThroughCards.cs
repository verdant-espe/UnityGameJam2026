using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FlipThroughCards : MonoBehaviour
{
    // List of five cards to flip through
    public List<GameObject> cards;

    // Card rigidbodies
    private Rigidbody rb;

    // How long it takes to flip through every card
    private float rotationForce;

    // How long it takes for the rotation to stop and select a card
    private float stopForce;

    // How long it takes for cards to start/stop rotation
    private float t;

    // Cards are currently in rotation
    int inRotate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Assigns rb to rigidbody component
        rb = GetComponent<Rigidbody>();

        StartCardFlip();
    }


    // Update is called once per frame
    private void Update()
    {

    }

    // Upon touching a card platform, an animation plays that flips through the set of five cards before landing on one, giving the player a buff + debuff effect
    void StartCardFlip()
    {
        foreach(var card in cards)
        {

        }
    }

    void StopCardFlip()
    {

    }
}
