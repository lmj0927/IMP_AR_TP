using System.Collections;
using UnityEngine;

public class BatterySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private int targetCount = 5;
    [SerializeField] private int maxTryCount = 30;

    [Header("Range Settings (Local)")]
    [SerializeField] private float rangeX = 1.5f;
    [SerializeField] private float rangeZ = 1.5f;
    [SerializeField] private float spawnY = 0.05f;
    [SerializeField] private float checkRadius = 0.2f; // 배터리 크기에 맞춰 조정

    private bool _isInitialized = false;

    public void InitializeSpawning()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        
        StartCoroutine(InitialSpawnRoutine());
    }

    private IEnumerator InitialSpawnRoutine()
    {
        int currentCount = 0;
        while (currentCount < targetCount)
        {
            if (TryGetValidPosition(out Vector3 localPos))
            {
                GameObject battery = Instantiate(batteryPrefab, transform);
                battery.transform.localPosition = localPos;
                battery.transform.localRotation = Quaternion.identity;
                currentCount++;
            }
            yield return null; // 프레임 부하 방지
        }
    }

    public void RequestRespawn(BatteryItem item, float minDelay, float maxDelay)
    {
        StartCoroutine(RespawnRoutine(item, minDelay, maxDelay));
    }

    private IEnumerator RespawnRoutine(BatteryItem item, float minDelay, float maxDelay)
    {
        bool placed = false;
        while (!placed)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            // 위치를 찾을 때까지 maxTryCount만큼 시도
            for (int i = 0; i < maxTryCount; i++)
            {
                if (TryGetValidPosition(out Vector3 localPos, item.gameObject))
                {
                    item.transform.localPosition = localPos;
                    item.Show();
                    placed = true;
                    break;
                }
            }
            // 실패했다면 다음 루프(delay 후)에서 다시 시도함
            if (!placed) Debug.LogWarning($"{item.name} 위치 찾기 실패. 재시도 예정.");
        }
    }

    private bool TryGetValidPosition(out Vector3 localPos, GameObject ignoreObject = null)
    {
        float rx = Random.Range(-rangeX, rangeX);
        float rz = Random.Range(-rangeZ, rangeZ);
        localPos = new Vector3(rx, spawnY, rz);

        Vector3 worldPos = transform.TransformPoint(localPos);
        Collider[] hitColliders = Physics.OverlapSphere(worldPos, checkRadius);

        foreach (var col in hitColliders)
        {
            if (col.CompareTag("Battery") && col.gameObject != ignoreObject)
            {
                return false; // 다른 배터리가 이미 있음
            }
        }
        return true;
    }
}