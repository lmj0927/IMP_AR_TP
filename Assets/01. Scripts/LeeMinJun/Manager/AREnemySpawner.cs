using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AREnemySpawner : MonoBehaviour
{
    private ARTrackedImageManager trackedImgManager;
    [SerializeField] private List<GameObject> enemyPrefabs;

    private GameObject enemy;

    void Start()
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
                SpawnEnemy();
            }
        }

        foreach (var trackedImage in args.updated)
        {
            if (trackedImage.referenceImage.name == "Enemy" && trackedImage.trackingState == TrackingState.Tracking && enemy == null)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        var randX = Random.Range(-10, 10);
        var randZ = Random.Range(-10, 10);
        var offset = new Vector3(randX, 0, randZ);
        enemy = Instantiate(enemyPrefabs[Mathf.Min(GameManager.Instance.GetStageLevel(), enemyPrefabs.Count - 1 )], Camera.main.transform.position + offset, Quaternion.identity);
    }
}
