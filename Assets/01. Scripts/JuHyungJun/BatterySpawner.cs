using UnityEngine;

public class BatterySpawner : MonoBehaviour
{
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private int targetCount = 5;
    [SerializeField] private int maxTryCount = 30;

    [SerializeField] private float rangeX = 2f;
    [SerializeField] private float rangeZ = 2f;
    [SerializeField] private float spawnY = 0.1f;
    [SerializeField] private float checkRadius = 0.5f;

    void Start()
    {
        SpawnBatteries();
    }

    private void SpawnBatteries()
    {
        int currentCount = 0;
        int tryCount = 0;

        while (currentCount < targetCount && tryCount < maxTryCount)
        {
            tryCount++;

            float randomX = Random.Range(-rangeX, rangeX);
            float randomZ = Random.Range(-rangeZ, rangeZ);

            Vector3 localPosition = new Vector3(randomX, spawnY, randomZ);
            Vector3 worldPosition = transform.TransformPoint(localPosition);

            Collider[] hitColliders = Physics.OverlapSphere(worldPosition, checkRadius);

            bool canSpawn = true;

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].CompareTag("Battery"))
                {
                    canSpawn = false;
                    break;
                }
            }

            if (canSpawn)
            {
                GameObject battery = Instantiate(batteryPrefab, transform);
                battery.transform.localPosition = localPosition;
                battery.transform.localRotation = Quaternion.identity;

                currentCount++;
            }
        }

        Debug.Log("생성된 배터리 수 : " + currentCount);
        Debug.Log("총 시도 횟수 : " + tryCount);
    }
}