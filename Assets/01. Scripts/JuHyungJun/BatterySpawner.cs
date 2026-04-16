using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This script manages the procedural spawning of battery items in the AR environment.
//It handles ground detection, overlap prevention, and independent respawn timers for each item.
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
    
    // Tracks active respawn routines to avoid duplicate coroutines for the same object
    private readonly Dictionary<BatteryItem, Coroutine> respawnCoroutines = new();

    private Vector3 spawnCenter;
    private bool hasSpawnedOnce = false;

    private void Update()
    {
        // Continuously check for ground detection until the first batch of batteries is spawned
        if (!hasSpawnedOnce)
        {
            TrySpawnAllBatteries();
        }
    }
    
    // Attempts to find an AR plane and instantiate the set amount of batteries
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

            // Link the collection event to the handle function for respawning
            battery.OnCollected += HandleBatteryCollected;
            spawnedBatteries.Add(battery);
        }

        hasSpawnedOnce = true;
    }

    // Called when a battery is deactivated; initiates the respawn sequence
    private void HandleBatteryCollected(BatteryItem battery)
    {
        if (battery == null)
            return;
        
        // Stop any existing coroutine for this specific battery before starting a new one
        if (respawnCoroutines.TryGetValue(battery, out Coroutine oldCoroutine))
        {
            StopCoroutine(oldCoroutine);
            respawnCoroutines.Remove(battery);
        }

        Coroutine coroutine = StartCoroutine(RespawnRoutine(battery));
        respawnCoroutines.Add(battery, coroutine);
    }

    // Waits for a delay, updates ground data, and reactivates the battery at a new location
    private IEnumerator RespawnRoutine(BatteryItem battery)
    {
        yield return new WaitForSeconds(respawnDelay);

        if (battery == null)
            yield break;
        
        // Refresh the ground center if the AR environment has updated
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

    // Finds a spawn point that meets the minimum distance requirements from other active batteries
    private Vector3 GetValidSpawnPosition()
    {
        // Try up to 30 times to find a valid non-overlapping position
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
        
        // Default fallback if no valid position is found after 30 attempts
        return spawnCenter + Vector3.up * spawnHeightOffset;
    }

    // Validation logic to ensure batteries are not spawned too close to each other
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