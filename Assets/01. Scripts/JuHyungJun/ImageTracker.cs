using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private GameObject trackerObjectPrefab;

    private bool hasSpawned = false;

    private void OnEnable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
        }
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
        }
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        if (hasSpawned)
            return;

        foreach (ARTrackedImage trackedImage in args.added)
        {
            TryPlaceTracker(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in args.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                TryPlaceTracker(trackedImage);
            }
        }
    }

    private void TryPlaceTracker(ARTrackedImage trackedImage)
    {
        if (hasSpawned)
            return;

        hasSpawned = true;

        Instantiate(
            trackerObjectPrefab,
            trackedImage.transform.position,
            trackedImage.transform.rotation
        );

        trackedImageManager.enabled = false;

        Debug.Log("이미지 인식 완료 - TrackerObject 생성 후 트래킹 종료");
    }
}