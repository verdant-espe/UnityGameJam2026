
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ScrollingBackground : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Texture2D backgroundTexture;

    [Header("Background Settings")]
    [SerializeField] private float distanceFromCamera = 30f;
    [SerializeField] private float scrollSpeed = 0.015f;
    [SerializeField] private Vector2 textureTiling = new Vector2(1f, 2f);

    private Material backgroundMaterial;
    private Texture2D temporaryTexture;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
        {
            Debug.LogError("Scrolling Background needs a camera!");
            enabled = false;
            return;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");

        if (shader == null)
        {
            Debug.LogError("URP Unlit shader was not found!");
            enabled = false;
            return;
        }

        backgroundMaterial = new Material(shader);

        // Use actual artwork when assigned.
        // Otherwise, create a temporary striped sky.
        if (backgroundTexture == null)
        {
            temporaryTexture = new Texture2D(8, 64);

            for (int y = 0; y < 64; y++)
            {
                float wave = Mathf.Sin(y * Mathf.PI * 2f / 64f);
                float blend = (wave + 1f) * 0.5f;

                Color color = Color.Lerp(
                    new Color(0.12f, 0.28f, 0.52f),
                    new Color(0.42f, 0.70f, 0.90f),
                    blend
                );

                for (int x = 0; x < 8; x++)
                    temporaryTexture.SetPixel(x, y, color);
            }

            temporaryTexture.wrapMode = TextureWrapMode.Repeat;
            temporaryTexture.Apply();

            backgroundTexture = temporaryTexture;
        }

        backgroundMaterial.SetTexture("_BaseMap", backgroundTexture);
        backgroundMaterial.SetTextureScale("_BaseMap", textureTiling);

        GetComponent<MeshRenderer>().material = backgroundMaterial;
    }

    void LateUpdate()
    {
        if (targetCamera == null || backgroundMaterial == null)
            return;

        float distance = Mathf.Max(1f, distanceFromCamera);

        // Keep the background centered behind the camera.
        transform.position =
            targetCamera.transform.position +
            targetCamera.transform.forward * distance;

        transform.rotation = targetCamera.transform.rotation;

        // Cover the camera's entire visible area.
        float viewHeight = targetCamera.orthographic
            ? targetCamera.orthographicSize * 2f
            : 2f * distance * Mathf.Tan(
                targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad
            );

        float viewWidth = viewHeight * targetCamera.aspect;

        transform.localScale = new Vector3(
            viewWidth * 1.05f,
            viewHeight * 1.05f,
            1f
        );

        // Scroll based on camera height, including falling.
        float offsetY = targetCamera.transform.position.y * scrollSpeed;

        backgroundMaterial.SetTextureOffset(
            "_BaseMap",
            new Vector2(0f, offsetY)
        );
    }

    void OnDestroy()
    {
        if (backgroundMaterial != null)
            Destroy(backgroundMaterial);

        if (temporaryTexture != null)
            Destroy(temporaryTexture);
    }
}
