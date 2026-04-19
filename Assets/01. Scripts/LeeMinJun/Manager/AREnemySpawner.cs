using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AREnemySpawner : MonoBehaviour
{
    private ARTrackedImageManager trackedImgManager;
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private GameObject enemySpawnVFX;

    private GameObject enemy;

    void Awake()
    {
        trackedImgManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        trackedImgManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    void OnDisable()
    {
        trackedImgManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var trackedImage in args.added)
        {
            Debug.Log(trackedImage.referenceImage.name);
            if (trackedImage.referenceImage.name == "Enemy")
            {
                SpawnEnemy(trackedImage);
            }
        }

        foreach (var trackedImage in args.updated)
        {
            if (trackedImage.referenceImage.name == "Enemy" && trackedImage.trackingState == TrackingState.Tracking && enemy == null)
            {
                SpawnEnemy(trackedImage);
            }
        }
    }

    private void SpawnEnemy(ARTrackedImage trackedImage)
    {
        if (enemy != null) return;
        Instantiate(enemySpawnVFX, trackedImage.transform.position, trackedImage.transform.rotation);
        var rand = Random.Range(10f, 20f);
        var cam = Camera.main;
        enemy = Instantiate(enemyPrefabs[Mathf.Min(GameManager.Instance.GetStageLevel(), enemyPrefabs.Count - 1 )], cam.transform.position + cam.transform.forward * rand, Quaternion.identity);
        
    }
}
