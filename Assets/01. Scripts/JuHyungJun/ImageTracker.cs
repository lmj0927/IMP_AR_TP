using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("디버그 모드: 강제로 배터리 스폰을 시작합니다.");
        
            BatterySpawner spawner = FindObjectOfType<BatterySpawner>();

            if (spawner != null)
            {
                spawner.InitializeSpawning();
            }
            else
            {
                // 배터리 스포너를 못 찾았을 때 출력되는 경고
                Debug.LogError("씬에 BatterySpawner가 없습니다! 테스트용 배터리 프리팹을 씬에 직접 끌어다 놓았는지 확인해 주세요.");
            }
        }
    }
    
    private void OnEnable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage image in args.added)
        {
            // 이미지 프리팹에 BatterySpawner가 붙어있다고 가정
            BatterySpawner spawner = image.GetComponentInChildren<BatterySpawner>();
            if (spawner != null)
            {
                Debug.Log($"[AR] {image.referenceImage.name} 감지됨. 배터리 생성을 시작합니다.");
                spawner.InitializeSpawning();
            }
        }

        // 이미지가 업데이트될 때 (추적 상태 변화 등) 필요한 로직이 있다면 여기에 추가
        foreach (ARTrackedImage image in args.updated)
        {
            if (image.trackingState == TrackingState.Tracking)
            {
                // 다시 보이기 시작했을 때의 로직 등
            }
        }
    }
}