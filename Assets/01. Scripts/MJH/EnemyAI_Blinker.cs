using UnityEngine;
using System.Collections;

public class EnemyAI_Blinker : MonoBehaviour
{
    [Header("점멸자: 원거리 사격 설정")]
    public GameObject projectilePrefab; // 방금 만든 GhostOrb 프리팹을 넣을 칸
    public float teleportInterval = 3.0f; // 공격과 공격 사이의 숨돌릴 틈
    public float spawnDistance = 4.0f;    
    public float projectileSpeed = 4.0f;  // 구체가 날아가는 속도 (피할 수 있는 속도)
    public float projectileDamage = 15f;

    [Header("시각적 기괴함")]
    public float floatSpeed = 1.5f;
    public float floatHeight = 0.2f;

    private Transform playerCamera;
    private PlayerHealth targetPlayer;
    private bool isBusy = false;
    private float basePosY;

    void Start()
    {
        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
            targetPlayer = playerCamera.GetComponent<PlayerHealth>();
        }
        basePosY = transform.position.y;
        StartCoroutine(SniperRoutine()); // 시작하자마자 사격 루틴 돌입
    }

    void Update()
    {
        if (playerCamera != null)
        {
            Vector3 lookDir = playerCamera.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3.0f);
            }

            if (!isBusy) ApplyFloatingEffect();
        }
    }

    void ApplyFloatingEffect()
    {
        float newY = basePosY + (Mathf.Sin(Time.time * floatSpeed) * floatHeight);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    IEnumerator SniperRoutine()
    {
        yield return new WaitForSeconds(2.0f);

        while (true)
        {
            // 1. 시야에서 사라지고 사각지대로 이동 (점멸)
            yield return StartCoroutine(Teleport());

            // 2. 나타나자마자 기를 모아 구체를 던짐
            yield return StartCoroutine(ChargeAndShoot());

            // 3. 다음 점멸 전까지 대기 (플레이어가 반격할 타이밍)
            yield return new WaitForSeconds(teleportInterval);
        }
    }

    IEnumerator Teleport()
    {
        isBusy = true;
        Debug.Log("<color=magenta>[점멸]</color> 원혼이 등 뒤로 숨어들었습니다!");

        // 10미터 땅속으로 꺼져서 사라진 척
        transform.position += Vector3.down * 10f;
        yield return new WaitForSeconds(1.5f);

        // 플레이어 반경 360도, 4m 거리에 무작위 배치
        float randomAngle = Random.Range(0, 360) * Mathf.Deg2Rad;
        Vector3 randomDir = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));

        Vector3 newPos = playerCamera.position + (randomDir * spawnDistance);
        newPos.y = playerCamera.position.y;
        basePosY = newPos.y;
        
        transform.position = newPos;

        isBusy = false;
    }

    IEnumerator ChargeAndShoot()
    {
        isBusy = true;
        Debug.Log("<color=orange>[조준]</color> 원혼이 불덩이를 모으고 있습니다!");

        // 1. 귀신 코앞에 구체 소환
        Vector3 spawnPos = transform.position + (transform.forward * 0.5f);
        GameObject orb = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // 2. 1.5초간 기 모으기 (투사체가 0.2배율까지 서서히 커짐)
        float chargeTime = 1.5f;
        float elapsed = 0f;
        while (elapsed < chargeTime)
        {
            if (orb != null)
            {
                orb.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 0.2f, elapsed / chargeTime);
                orb.transform.position = transform.position + (transform.forward * 0.5f); // 귀신 손에 계속 붙어있음
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("<color=red>[발사!]</color> 구체가 날아옵니다! 옆으로 물리적으로 피하십시오!");

        // 3. 발사 순간, 플레이어의 '현재 위치'를 타겟팅하고 발사 코루틴에 넘김
        if (orb != null && playerCamera != null)
        {
            Vector3 moveDirection = (playerCamera.position - orb.transform.position).normalized;
            StartCoroutine(FlyProjectile(orb, moveDirection));
        }

        isBusy = false;
    }

    IEnumerator FlyProjectile(GameObject orb, Vector3 direction)
    {
        float lifeTime = 3.0f; // 최대 3초 날아감
        float elapsed = 0f;

        while (elapsed < lifeTime && orb != null)
        {
            // 투사체가 지정된 방향으로 일직선으로 날아감 (유도탄 아님)
            orb.transform.position += direction * projectileSpeed * Time.deltaTime;

            // 투사체와 플레이어(카메라) 간의 거리가 0.5m 이내로 좁혀지면 피격 판정
            if (Vector3.Distance(orb.transform.position, playerCamera.position) < 0.5f)
            {
                Debug.Log("<color=red>[피격]</color> 원혼의 구체에 맞았습니다!");
                if (targetPlayer != null) targetPlayer.TakeDamage(projectileDamage);
                Destroy(orb); // 맞으면 구체 소멸
                yield break; 
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 빗나가서 시간이 다 되면 허공에서 소멸
        if (orb != null) Destroy(orb);
    }

    public void BlinkAwayOnHit()
    {
        // 총에 맞으면 즉시 하던 행동을 멈추고 다른 곳으로 점멸 (생존기)
        if (!isBusy)
        {
            StopAllCoroutines();
            StartCoroutine(TeleportThenResume());
        }
    }

    IEnumerator TeleportThenResume()
    {
        yield return StartCoroutine(Teleport());
        StartCoroutine(SniperRoutine());
    }
}