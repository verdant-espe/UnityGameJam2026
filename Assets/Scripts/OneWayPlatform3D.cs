
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OneWayPlatform3D : MonoBehaviour
{
    [SerializeField] private float clearance = 0.15f;

    private BoxCollider platformCollider;

    // Each platform remembers its collision state
    // for every character it encounters.
    private readonly Dictionary<CharacterController, bool>
        ignoredCharacters = new Dictionary<CharacterController, bool>();

    // Shared character list, refreshed periodically.
    // This avoids searching the whole scene separately
    // for every generated platform.
    private static CharacterController[] allCharacters =
        new CharacterController[0];

    private static float nextSearchTime = -1f;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetCache()
    {
        allCharacters = new CharacterController[0];
        nextSearchTime = -1f;
    }

    void Awake()
    {
        platformCollider = GetComponent<BoxCollider>();
    }

    void OnDisable()
    {
        ignoredCharacters.Clear();
    }

    void Update()
    {
        // Refresh the global character list about
        // four times per second.
        // This also discovers newly spawned enemies.
        if (Time.time >= nextSearchTime)
        {
            allCharacters = FindObjectsByType<CharacterController>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            nextSearchTime = Time.time + 0.25f;
        }

        float platformTop = platformCollider.bounds.max.y;

        foreach (CharacterController character in allCharacters)
        {
            if (character == null)
                continue;

            // Inactive enemies will be handled when spawned.
            if (!character.enabled || !character.gameObject.activeInHierarchy)
            {
                ignoredCharacters.Remove(character);
                continue;
            }

            float feetY = character.bounds.min.y;

            // Set the correct collision state for
            // a character we have not handled before.
            if (!ignoredCharacters.TryGetValue(
                character, out bool isIgnoring))
            {
                isIgnoring = feetY < platformTop - clearance;

                Physics.IgnoreCollision(
                    character,
                    platformCollider,
                    isIgnoring
                );

                ignoredCharacters[character] = isIgnoring;

                continue;
            }

            // Character has moved completely above the platform.
            // Restore collision so it can land.
            if (isIgnoring &&
                feetY > platformTop + clearance)
            {
                Physics.IgnoreCollision(
                    character,
                    platformCollider,
                    false
                );

                ignoredCharacters[character] = false;
            }

            // Character is underneath the platform.
            // Let it pass upward through the bottom.
            else if (!isIgnoring &&
                     feetY < platformTop - clearance)
            {
                Physics.IgnoreCollision(
                    character,
                    platformCollider,
                    true
                );

                ignoredCharacters[character] = true;
            }
        }
    }
}
