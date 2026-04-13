using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySpawner : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private BatteryItem batteryPrefab;
    [SerializeField] private ARGroundResolver groundResolver;

    [Header("Spawn Settings")]
    [SerializeField] private int spawnCount = 5;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private float spawnHeightOffset = 0.8f;
    [SerializeField] private float minDistanceBetweenBatteries = 0.7f;

    private readonly List<BatteryItem> spawnedBatteries = new();
    private readonly Dictionary<BatteryItem, Coroutine> respawnCoroutines = new();

    private Vector3 spawnCenter;
    private bool hasSpawnedOnce = false;

    private void Update()
    {
        if (!hasSpawnedOnce)
        {
            TrySpawnAllBatteries();
        }
    }

    public void TrySpawnAllBatteries()
    {
        if (!groundResolver.TryResolveGround(out Vector3 groundPosition))
        {
            return;
        }

        spawnCenter = groundPosition;
        Debug.Log($"<color=cyan>바닥 감지 성공! 좌표: {spawnCenter} 주변에 스폰 시작</color>");

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();

            BatteryItem battery = Instantiate(
                batteryPrefab,
                spawnPosition,
                Quaternion.identity,
                transform
            );

            battery.OnCollected += HandleBatteryCollected;
            spawnedBatteries.Add(battery);
        }

        hasSpawnedOnce = true;
    }

    private void HandleBatteryCollected(BatteryItem battery)
    {
        if (battery == null)
            return;

        if (respawnCoroutines.TryGetValue(battery, out Coroutine oldCoroutine))
        {
            StopCoroutine(oldCoroutine);
            respawnCoroutines.Remove(battery);
        }

        Coroutine coroutine = StartCoroutine(RespawnRoutine(battery));
        respawnCoroutines.Add(battery, coroutine);
    }

    private IEnumerator RespawnRoutine(BatteryItem battery)
    {
        yield return new WaitForSeconds(respawnDelay);

        if (battery == null)
            yield break;

        if (groundResolver.TryResolveGround(out Vector3 groundPosition))
        {
            spawnCenter = groundPosition;
        }

        battery.transform.position = GetValidSpawnPosition();
        battery.gameObject.SetActive(true);

        if (respawnCoroutines.ContainsKey(battery))
        {
            respawnCoroutines.Remove(battery);
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        for (int tryCount = 0; tryCount < 30; tryCount++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;

            Vector3 candidatePosition = new Vector3(
                spawnCenter.x + randomCircle.x,
                spawnCenter.y + spawnHeightOffset,
                spawnCenter.z + randomCircle.y
            );

            if (IsPositionValid(candidatePosition))
            {
                return candidatePosition;
            }
        }

        return spawnCenter + Vector3.up * spawnHeightOffset;
    }

    private bool IsPositionValid(Vector3 candidatePosition)
    {
        for (int i = 0; i < spawnedBatteries.Count; i++)
        {
            BatteryItem battery = spawnedBatteries[i];

            if (battery == null || !battery.gameObject.activeSelf)
                continue;

            float distance = Vector3.Distance(candidatePosition, battery.transform.position);

            if (distance < minDistanceBetweenBatteries)
            {
                return false;
            }
        }

        return true;
    }
}