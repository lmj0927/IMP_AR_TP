using UnityEngine;
using System.Collections;

public class TrueARAutoSpawner : MonoBehaviour
{
    [Header("소환 설정")]
    public GameObject enemyPrefab;
    public float targetDistance = 2.0f; // 플레이어로부터 떨어질 거리

    private Transform playerCamera;
    private bool hasSpawned = false;

    void Start()
    {
        if (Camera.main != null) playerCamera = Camera.main.transform;
        
        // 시작하자마자 카운트다운 시작
        StartCoroutine(AutoSpawnRoutine());
    }

    IEnumerator AutoSpawnRoutine()
    {
        // 1. [대기] 3~5초 사이 무작위 시간
        float waitTime = Random.Range(3.0f, 5.0f);
        Debug.Log($"<color=yellow>[알림]</color> {waitTime:F1}초 뒤, 원혼이 당신의 정면에 나타납니다. 주변을 스캔하십시오!");
        
        yield return new WaitForSeconds(waitTime);

        if (hasSpawned || playerCamera == null) yield break;

        // 2. [좌표 계산] 카메라의 현재 정면(Forward) 방향으로 2m 지점
        Vector3 spawnDirection = playerCamera.forward;
        spawnDirection.y = 0; // 수평 유지
        spawnDirection.Normalize();

        // 현재 카메라 높이(눈높이)를 그대로 유지한 채 2m 앞 좌표 생성
        Vector3 spawnPos = playerCamera.position + (spawnDirection * targetDistance);

        // 3. [소환]
        GameObject ghost = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        
        // 플레이어를 바라보게 회전
        Vector3 lookAtPlayer = playerCamera.position - ghost.transform.position;
        lookAtPlayer.y = 0;
        ghost.transform.rotation = Quaternion.LookRotation(lookAtPlayer);

        // 4. [독립] 카메라의 자식으로 들어가지 않게 설정 (화면 귀속 방지)
        ghost.transform.SetParent(null);

        hasSpawned = true;
        Debug.Log("<color=red>[자동 강림]</color> 원혼이 허공에 고정되었습니다. 이제 움직여서 피하십시오!");
    }
}