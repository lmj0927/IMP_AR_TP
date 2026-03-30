using UnityEngine;

public class BatteryItem : MonoBehaviour
{
    [SerializeField] private float minRespawnTime = 40f;
    [SerializeField] private float maxRespawnTime = 80f;

    private BatterySpawner _batterySpawner;
    private Collider _batteryCollider;
    private Renderer[] _batteryRenderers;
    private bool _isCollected = false;

    private void Awake()
    {
        // 부모 오브젝트에서 Spawner를 찾음
        _batterySpawner = GetComponentInParent<BatterySpawner>();
        _batteryCollider = GetComponent<Collider>();
        _batteryRenderers = GetComponentsInChildren<Renderer>(true);
    }

    public void Collect()
    {
        if (_isCollected) return;

        _isCollected = true;
        SetVisible(false);

        if (_batterySpawner != null)
        {
            // Spawner에게 리스폰을 요청함
            _batterySpawner.RequestRespawn(this, minRespawnTime, maxRespawnTime);
        }
    }

    public void Show()
    {
        _isCollected = false;
        SetVisible(true);
    }

    private void SetVisible(bool isVisible)
    {
        if (_batteryCollider != null) _batteryCollider.enabled = isVisible;
        foreach (var rend in _batteryRenderers)
        {
            rend.enabled = isVisible;
        }
    }
}