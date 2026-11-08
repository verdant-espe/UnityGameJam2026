using UnityEngine;

public class CardSystem : MonoBehaviour
{
    // Triple Jump card
    public GameObject card;

    // References player
    public PlayerMovement player;

    // References enemy
    public EnemyChase enemy;

    // Checks if player has collected a card
    public bool hasCard;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Upon entering trigger on card platform, player now has a card
        if(other.CompareTag("Card"))
        {
            hasCard = true; 
        }

        // If player gets Card1, player gains a third jump but enemy gets faster
        if(other.gameObject.name == "Card1")
        {
            player.maxJumps = 3;

            enemy.speed = 6;
        }


        // We don't have the code for projectiles, health, or damage yet so I'm gonna wait on those to finish this up


        // If player gets Card2, projectile damage is doubled but damage received is doubled as well
        if (other.gameObject.name == "Card2")
        {

        }

        // If player gets Card3, projectile fire rate is increased but speed is decreased
        if (other.gameObject.name == "Card3")
        {
            player.speed = 3f;
        }

        // If player gets Card4, projectile gets increased knockback but a reduced fire rate
        if(other.gameObject.name == "Card4")
        {
        
        }

        // If player gets Card5, health is regenerated but enemy moves faster
        if (other.gameObject.name == "Card5")
        {
            enemy.speed = 6;
        }

        // If player gets Card6, speed is increased but air control is reduced
        if (other.gameObject.name == "Card6")
        {
            player.speed = 8f;

            player.gravity = 10;
        }
    }
}
