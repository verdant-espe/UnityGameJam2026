
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform aimPivot;
    [SerializeField] private SpriteRenderer armSpriteRenderer;
    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        if (mainCamera == null || aimPivot == null)
        {
            return;
        }

        // Mouse position converted to a world-space ray.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Intersect the ray with the XY plane at the pivot's depth.
        Plane aimPlane = new Plane(
            Vector3.forward,
            aimPivot.position
        );

        float distance;

        if (aimPlane.Raycast(ray, out distance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(distance);

            // Direction from shoulder to mouse.
            Vector3 direction =
                mouseWorldPosition - aimPivot.position;

            // Rotate around the Z axis.
            float angle = Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg;

            aimPivot.rotation = Quaternion.Euler(0f, 0f, angle);

            // Keep the finger pistol's thumb facing upward.
            if (direction.x < 0f)
            {
                armSpriteRenderer.flipY = true;
            }
            else
            {
                armSpriteRenderer.flipY = false;
            }
        }
    }
}
