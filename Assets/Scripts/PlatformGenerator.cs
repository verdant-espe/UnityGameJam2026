using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float previousX = 0f;
        float previousY = 0f;

        for (int i = 1; i <= 12; i++)
        {
            float spawnY = i * 3f;
            float randomX = Mathf.Clamp(previousX + Random.Range(-2f, 2f), -3f, 3f);

            Instantiate(platformPrefab, new Vector3(randomX, spawnY, 0f), Quaternion.identity);

            previousX = randomX;
        }
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
