using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AREnemySpawner : MonoBehaviour
{
    private ARTrackedImageManager trackedImgManager;
    [SerializeField] private GameObject objectToInstantiate;
    

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
                var randX = Random.Range(-10, 10);
                var randZ = Random.Range(-10, 10);
                var offset = new Vector3(randX, 0, randZ);
                Instantiate(objectToInstantiate, Camera.main.transform.position + offset, Quaternion.identity);
            }
        }
    }
}
