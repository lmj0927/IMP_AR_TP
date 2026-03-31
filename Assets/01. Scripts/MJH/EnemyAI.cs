using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("추적 및 공격 설정")]
    public float moveSpeed = 1.0f;      // 이동 속도 (초당 1미터 이동)
    public float attackRange = 2.0f;    // 공격 사거리 (카메라와 이 거리보다 가까워지면 멈춤)
    public float attackCooldown = 2.0f; // 공격 쿨타임 (2초마다 한 번씩 공격)
    private PlayerHealth targetPlayer;    private Transform playerCamera;     // 플레이어의 눈(AR 카메라) 위치
    private float lastAttackTime;       // 마지막으로 공격한 시간 기록

    void Start()
    {
        // AR 환경에서 플레이어는 곧 'Main Camera'입니다.
        // 게임 시작 시 카메라의 위치를 찾아 기억해 둡니다.
        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
            
            // [수정된 핵심 로직] 카메라에 붙어있는 PlayerHealth 컴포넌트를 찾아옵니다.
            targetPlayer = playerCamera.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogError("치명적 에러: 씬에 MainCamera 태그가 붙은 카메라가 없습니다!");
        }

    }

    void Update()
    {
        // 카메라를 못 찾았다면 아무 행동도 하지 않음
        if (playerCamera == null) return;

        // 1. 적(자신)과 플레이어(카메라) 사이의 실제 물리적 거리(미터) 계산
        float distanceToPlayer = Vector3.Distance(transform.position, playerCamera.position);

        // 2. 상태 판단: 사거리 밖인가, 안인가?
        if (distanceToPlayer > attackRange)
        {
            ChasePlayer(); // 멀면 쫓아간다
        }
        else
        {
            AttackPlayer(); // 가까우면 멈춰서 때린다
        }
    }

    void ChasePlayer()
    {
        // 플레이어(카메라)를 정면으로 바라보게 회전시킵니다.
        transform.LookAt(playerCamera);
        
        // 바라보는 방향(앞)으로 지정된 속도만큼 이동합니다.
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // [수정된 핵심 로직] 단순히 로그만 띄우는 게 아니라, 진짜로 데미지를 10 깎습니다.
            if (targetPlayer != null)
            {
                targetPlayer.TakeDamage(10f);
            }
            else
            {
                Debug.LogWarning("때리려고 했는데 플레이어한테 PlayerHealth 스크립트가 없습니다!");
            }
            
            lastAttackTime = Time.time; 
        }
    }
}