
using System;
using System.Collections;
using UnityEngine;

public class D100RollAnimation : MonoBehaviour
{
    [Header("Dice Animation")]
    [SerializeField] private float rollDuration = 2.5f;
    [SerializeField] private float horizontalTravel = 5f;
    [SerializeField] private float bounceHeight = 0.75f;
    [SerializeField] private float spinSpeed = 720f;

    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private Coroutine activeRoll;

    void Awake()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }

    public void Roll(Action onFinished = null)
    {
        if (activeRoll != null)
        {
            StopCoroutine(activeRoll);
        }

        activeRoll = StartCoroutine(RollRoutine(onFinished));
    }

    private IEnumerator RollRoutine(Action onFinished)
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.1f, rollDuration);

        transform.position = startingPosition +
                             Vector3.left * horizontalTravel * 0.5f;
        transform.rotation = startingRotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            float x = Mathf.Lerp(
                -horizontalTravel * 0.5f,
                horizontalTravel * 0.5f,
                progress
            );

            float bounce = Mathf.Abs(
                Mathf.Sin(progress * Mathf.PI * 6f)
            ) * bounceHeight * (1f - progress);

            transform.position = startingPosition +
                new Vector3(x, bounce, 0f);


            Vector3 tumbleAxis = new Vector3(
                1f + 0.6f * Mathf.Sin(elapsed * 5f),
                0.7f + 0.8f * Mathf.Cos(elapsed * 6f),
                1.2f + 0.5f * Mathf.Sin(elapsed * 8f)
            ).normalized;

            float slowingSpin = Mathf.Lerp(1f, 0.1f, progress);

            transform.Rotate(
                tumbleAxis,
                spinSpeed * slowingSpin * Time.deltaTime,
                Space.Self
            );

            yield return null;
        }

        activeRoll = null;
        onFinished?.Invoke();
    }

    [ContextMenu("TEST: Roll D100")]
    private void TestRoll()
    {
        if (Application.isPlaying)
        {
            Roll();
        }
    }
}
