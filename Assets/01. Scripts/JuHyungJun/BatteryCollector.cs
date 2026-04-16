using UnityEngine;

// Attached to the Player/Camera to handle the logic for selecting and collecting items.
// Uses a Raycast from the viewport center (crosshair) to detect BatteryItem objects.

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
        // Update collection status and target item every frame
        canCollect = TryCollectBattery();
    }
    
    //method to be called by a UI Button to finalize collection
    public void CollectItem()
    {
        collectBattery.Collect();
    }

    
    // Raycasts from the screen center to check for batteries within a valid interaction range
    private bool TryCollectBattery()
    {
        // Generate ray from the middle of the screen (0.5, 0.5)
        Ray ray = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, batteryLayer))
            return false;

        if (hit.collider.TryGetComponent(out BatteryItem batteryItem))
        {
            // Check if the physical distance to the item is within the allowed 'collectDistance'
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
    
    // Returns current interaction status
    public bool GetCanCollect()
    {
        return canCollect;
    }
}