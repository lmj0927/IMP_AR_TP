using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class imgSpawn : MonoBehaviour
{
    [Header("소환 설정")]
    public GameObject enemyPrefab;
    private ARTrackedImageManager trackedImageManager;

    // 중복 소환을 막기 위한 살생부
    private Dictionary<string, GameObject> spawnedGhosts = new Dictionary<string, GameObject>();

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        if (trackedImageManager == null) Debug.LogError("치명적 에러: 이 오브젝트에 ARTrackedImageManager가 없습니다!");
    }

    void OnEnable() => trackedImageManager.trackedImagesChanged += OnChanged;
    void OnDisable() => trackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 1. 정상적으로 '새로 추가된' 이미지 감지 시
        foreach (var trackedImage in eventArgs.added)
        {
            TrySpawnGhost(trackedImage);
        }

        // 2. added를 건너뛰고 '업데이트'로 바로 넘어온 이미지 감지 시 (AR Foundation 버그 방어선)
        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                TrySpawnGhost(trackedImage);
            }
        }
    }

    void TrySpawnGhost(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        // 이미 소환된 개체라면 무시
        if (spawnedGhosts.ContainsKey(imageName)) return;

        // 적 생성
        Pose spawnPose = new Pose(trackedImage.transform.position, trackedImage.transform.rotation);
        GameObject ghost = Instantiate(enemyPrefab, spawnPose.position, spawnPose.rotation);

        // [핵심] 이미지 마커와의 부모-자식 관계를 강제로 끊어 카메라/마커 이동의 영향을 차단합니다.
        ghost.transform.SetParent(null);

        // 현실 좌표에 영구 고정 (못 박기)
        ghost.AddComponent<ARAnchor>();

        // 살생부에 등록
        spawnedGhosts.Add(imageName, ghost);

        Debug.Log($"<color=green>[이미지 소환 성공]</color> '{imageName}' 마커 위치에 원혼을 고정했습니다.");
    }
}