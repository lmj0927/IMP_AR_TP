using UnityEngine;

public class BatteryCollector : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Camera arCamera;

    [Header("Collect Settings")]
    [SerializeField] private float rayDistance = 20f;
    [SerializeField] private float collectDistance = 2f;
    [SerializeField] private LayerMask batteryLayer;

    private bool canCollect = false;
    private BatteryItem collectBattery;
    private void Update()
    {
        canCollect = TryCollectBattery();
#if UNITY_EDITOR
        /*if (Input.GetMouseButtonDown(0))
        {
            if (canCollect)
                collectBattery.Collect();
        }*/
#else
        // if (Input.touchCount > 0)
        // {
        //     Touch touch = Input.GetTouch(0);

        //     if (touch.phase == TouchPhase.Began)
        //     {
        //         TryCollectBattery();
        //     }
        // }
#endif
    }

    public void CollectItem()
    {
        collectBattery.Collect();
    }

    private bool TryCollectBattery()
    {
        Ray ray = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, batteryLayer))
            return false;

        if (hit.collider.TryGetComponent(out BatteryItem batteryItem))
        {
            float distanceToBattery = Vector3.Distance(
                arCamera.transform.position,
                batteryItem.transform.position
            );

            if (distanceToBattery > collectDistance)
                return false;

            
            collectBattery = batteryItem;
            //batteryItem.Collect();
            return true;
        }
        return false;
    }
    
    public bool GetCanCollect()
    {
        return canCollect;
    }
}