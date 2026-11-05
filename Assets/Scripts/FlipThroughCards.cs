using UnityEngine;
using System.Collections.Generic;
using Unity.XR.Oculus.Input;

public class FlipThroughCards : MonoBehaviour
{
    // List of five cards to flip through
    public List<GameObject> cards;

    // How long it takes to flip through every card before stopping
    private float flipTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Flips through cards for a total of 5 seconds
        flipTime = 5;

        StartCardFlip();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Upon touching a card platform, an animation plays that flips through the set of five cards before landing on one, giving the player a buff + debuff effect
    void StartCardFlip()
    {
        foreach(var card in cards)
        {

        }
    }
}
