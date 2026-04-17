using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("거리 경계선")]
    public float detectionRange = 10.0f;  
    public float attackRange = 1.5f;     
    
    [Header("속도 및 무빙 특성")]
    public float chaseSpeed = 2.0f;      
    public float floatSpeed = 2.0f;      
    public float floatHeight = 0.3f;     
    public float attackCooldown = 2.0f;
    public float dodgeSpeed = 1.5f;       
    public float dodgeInterval = 0.8f;   

    [Header("피격 피드백")]
    public float knockbackForce = 1.0f;     
    public float staggerDuration = 0.5f;    
    public float awakeDelay = 3.0f;      

    private float spawnTime;             
    private bool isAwake = false;        
    private bool isStaggered = false;    

    private Vector3 currentDodgeDirection;
    private float currentStrafeDir = 1f; // [신규 부품] 배회 방향을 결정하는 변수
    private float nextDodgeTime;
    private Transform playerCamera;
    private PlayerHealth targetPlayer;
    private float lastAttackTime;
    private bool isAttacking = false;

    void Start()
    {
        if (Camera.main != null) 
        {
            playerCamera = Camera.main.transform;
            targetPlayer = playerCamera.GetComponent<PlayerHealth>(); 
        }
        spawnTime = Time.time; 
    }

    void Update()
    {
        if (playerCamera == null || isAttacking || isStaggered) return;

        if (!isAwake)
        {
            SafeLookAt(); 
            ApplyFloatingEffect();          
            if (Time.time >= spawnTime + awakeDelay) isAwake = true;
            return; 
        }

        Vector2 myPos2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerPos2D = new Vector2(playerCamera.position.x, playerCamera.position.z);
        float distanceToPlayer = Vector2.Distance(myPos2D, playerPos2D);

        if (distanceToPlayer > attackRange) ChaseAndDodgePlayer(); 
        else EngagePlayer(); 
    }

    void SafeLookAt()
    {
        Vector3 lookDir = playerCamera.position - transform.position;
        lookDir.y = 0; 
        if (lookDir != Vector3.zero) 
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    public void ApplyKnockback(Vector3 hitDirection)
    {
        if (isAttacking) return; 
        if (!isAwake) isAwake = true;

        StopCoroutine("StaggerRoutine"); 
        StartCoroutine(StaggerRoutine(hitDirection));
    }

    private IEnumerator StaggerRoutine(Vector3 hitDirection)
    {
        isStaggered = true; 
        hitDirection.y = 0;
        hitDirection.Normalize();

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (hitDirection * knockbackForce);

        float elapsed = 0f;
        float pushDuration = 0.15f; 

        while (elapsed < pushDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / pushDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(Mathf.Max(0, staggerDuration - pushDuration));
        isStaggered = false; 
    }

    void ChaseAndDodgePlayer()
    {
        SafeLookAt();
        
        Vector3 forwardMove = transform.forward * chaseSpeed;

        if (Time.time >= nextDodgeTime)
        {
            float randomDir = Random.Range(-1f, 1f); 
            currentDodgeDirection = transform.right * (Mathf.Sign(randomDir) * dodgeSpeed);
            nextDodgeTime = Time.time + dodgeInterval;
        }

        transform.position += (forwardMove + currentDodgeDirection) * Time.deltaTime;
        ApplyFloatingEffect();
    }

    void EngagePlayer()
    {
        SafeLookAt();
        
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(JumpScareAttack());
            lastAttackTime = Time.time;
        }
        else
        {
            StrafeAroundPlayer();
        }
    }

    // [핵심 수술 부위] 나선형 추락을 막고 완벽한 1.5m 궤도를 강제합니다.
    void StrafeAroundPlayer()
    {
        if (Time.time >= nextDodgeTime)
        {
            currentStrafeDir = Random.value > 0.5f ? 1f : -1f; 
            nextDodgeTime = Time.time + dodgeInterval;
        }

        // 1. 플레이어를 중심축으로 삼아 정확히 원을 그리며 돕니다. (RotateAround)
        float orbitSpeed = dodgeSpeed * 25f; // 각속도 보정
        transform.RotateAround(playerCamera.position, Vector3.up, currentStrafeDir * orbitSpeed * Time.deltaTime);

        // 2. [절대 방어선] 원운동 중 발생하는 미세한 오차조차 허용하지 않도록, 거리를 1.5m(attackRange)로 강제 락(Lock) 겁니다.
        Vector3 offset = transform.position - playerCamera.position;
        offset.y = 0; // 평면 기준
        
        // 플레이어 위치 + (방향 벡터 * 정확히 1.5m)
        Vector3 lockedPos = playerCamera.position + (offset.normalized * attackRange);
        lockedPos.y = transform.position.y; // Y축 높이는 그대로 유지
        
        transform.position = lockedPos;

        ApplyFloatingEffect();
    }

    IEnumerator JumpScareAttack()
    {
        isAttacking = true; 
        Vector3 startAttackPos = transform.position;

        Debug.Log("<color=red>[공격]</color> 원혼이 쇄도합니다!");

        Vector3 targetMissilePos = playerCamera.position;
        float dashDuration = 0.2f; 
        float elapsed = 0f;
        
        while (elapsed < dashDuration)
        {
            transform.position = Vector3.Lerp(startAttackPos, targetMissilePos, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null; 
        }

        Vector2 attackEndPos2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerPos2D = new Vector2(playerCamera.position.x, playerCamera.position.z);
        float finalDist = Vector2.Distance(attackEndPos2D, playerPos2D);

        if (finalDist < 1.2f) 
        {
            Debug.Log("<color=red>[피격]</color> 데미지 적용!");
            if (targetPlayer != null) targetPlayer.TakeDamage();
        }

        yield return new WaitForSeconds(0.2f); 

        float returnDuration = 0.5f; 
        elapsed = 0f;
        Vector3 currentPos = transform.position; 

        while (elapsed < returnDuration)
        {
            transform.position = Vector3.Lerp(currentPos, startAttackPos, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isAttacking = false; 
    }

    void ApplyFloatingEffect()
    {
        float newY = transform.position.y + (Mathf.Sin(Time.time * floatSpeed) * floatHeight * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}