
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OneWayPlatform3D : MonoBehaviour
{
    [SerializeField] private float clearance = 0.15f;

    private BoxCollider platformCollider;
    private CharacterController player;
    private bool ignoringCollision;

    void Start()
    {
        platformCollider = GetComponent<BoxCollider>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogWarning("No Player-tagged object found.");
            enabled = false;
            return;
        }

        player = playerObject.GetComponent<CharacterController>();

        if (player == null)
        {
            Debug.LogWarning("Player needs a CharacterController.");
            enabled = false;
            return;
        }

        // Ignore the platform when starting underneath it.
        float feetY = player.bounds.min.y;
        float platformTop = platformCollider.bounds.max.y;

        ignoringCollision = feetY < platformTop - clearance;

        Physics.IgnoreCollision(
            player, platformCollider, ignoringCollision);
    }

    void Update()
    {
        if (player == null) return;

        float feetY = player.bounds.min.y;
        float platformTop = platformCollider.bounds.max.y;

        // Once completely above, allow landing.
        if (ignoringCollision && feetY > platformTop + clearance)
        {
            Physics.IgnoreCollision(player, platformCollider, false);
            ignoringCollision = false;
        }
        // Once below, allow jumping through.
        else if (!ignoringCollision && feetY < platformTop - clearance)
        {
            Physics.IgnoreCollision(player, platformCollider, true);
            ignoringCollision = true;
        }
    }
}
