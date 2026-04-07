using UnityEngine;

public class BatteryCollector : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Camera arCamera;

    [Header("Collect Settings")]
    [SerializeField] private float rayDistance = 20f;
    [SerializeField] private float collectDistance = 2f;
    [SerializeField] private LayerMask batteryLayer;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            TryCollectBattery();
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                TryCollectBattery();
            }
        }
#endif
    }

    private void TryCollectBattery()
    {
        Ray ray = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, batteryLayer))
            return;

        if (hit.collider.TryGetComponent(out BatteryItem batteryItem))
        {
            float distanceToBattery = Vector3.Distance(
                arCamera.transform.position,
                batteryItem.transform.position
            );

            if (distanceToBattery > collectDistance)
                return;

            Debug.Log("배터리 수집, 필터 시간 증가량: " + batteryItem.FilterTimeAmount);
            batteryItem.Collect();
        }
        
    }
}