using UnityEngine;

public class BatteryCollector : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private float collectDistance = 5f;
    [SerializeField] private LayerMask batteryLayer;
    [SerializeField] private Camera arCamera;

    private void Awake()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (arCamera == null) return;

        Ray ray = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * collectDistance, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, collectDistance, batteryLayer))
        {
            BatteryItem battery = hit.collider.GetComponentInParent<BatteryItem>();

            if (battery != null)
            {
                float distance = Vector3.Distance(ray.origin, hit.point);

                if (distance <= collectDistance)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Debug.Log($"배터리 획득! 거리: {distance:F2}m");
                        battery.Collect();
                    }
                }
            }
        }
    }
}