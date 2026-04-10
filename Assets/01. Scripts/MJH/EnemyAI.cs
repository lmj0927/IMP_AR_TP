using UnityEngine;
using System.Collections;
public class EnemyAI : MonoBehaviour
{
    [Header("거리 경계선 (Thresholds)")]
    public float detectionRange = 5.0f; 
    public float attackRange = 1.5f;     
    public float wanderRadius = 4.0f;    
    
    [Header("속도 및 특성")]
    public float wanderSpeed = 0.8f;   
    public float chaseSpeed = 2.0f;      
    public float floatSpeed = 2.0f;      
    public float floatHeight = 0.3f;     
    public float attackCooldown = 2.0f;
    
    [Header("전투(회피) 기동 설정")]
    public float dodgeSpeed = 2.5f;       
    public float dodgeInterval = 0.5f;   
    private Vector3 currentDodgeDirection;
    private float nextDodgeTime;

    private Transform playerCamera;
    private PlayerHealth targetPlayer;
    private Vector3 targetWanderPoint;
    private float lastAttackTime;
    private float originalY;
    private bool isAttacking = false;

    void Start()
    {
        if (Camera.main != null) 
        {
            playerCamera = Camera.main.transform;
            targetPlayer = playerCamera.GetComponent<PlayerHealth>(); 
        }
        SetNewWanderPoint();
    }

    void Update()
    {
        if (playerCamera == null || isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerCamera.position);

        // [핵심 논리 분해] 거리에 따른 3단계 행동 강제
        if (distanceToPlayer > detectionRange)
        {
            WanderAround(); 
        }
        else if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
        {
            ChasePlayer(); 
        }
       else
        {
            EngagePlayer(); 
        }
    }

    
    void WanderAround()
    {
        Vector3 direction = (targetWanderPoint - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetWanderPoint, wanderSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, targetWanderPoint) < 0.2f)
        {
            SetNewWanderPoint();
        }
        ApplyFloatingEffect();
    }

    void SetNewWanderPoint()
    {
        
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
        

        targetWanderPoint = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);
 
        targetWanderPoint.y = playerCamera.position.y; 
        
      
        originalY = targetWanderPoint.y; 
    }
  
    void ChasePlayer()
    {

        transform.LookAt(playerCamera);
        
       
        transform.position = Vector3.MoveTowards(transform.position, playerCamera.position, chaseSpeed * Time.deltaTime);
        
        ApplyFloatingEffect();
    }

    void ApplyFloatingEffect()
    {
        float newY = transform.position.y + (Mathf.Sin(Time.time * floatSpeed) * floatHeight * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void EngagePlayer()
    {
        
        transform.LookAt(playerCamera);


        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log($"<color=red>[발견 및 공격!]</color> 원혼이 틈을 노려 점프 스케어를 시전합니다!");
            StartCoroutine(JumpScareAttack());
            lastAttackTime = Time.time;
        }
      
        else
        {
            DodgeMovement();
        }
    }

    void DodgeMovement()
    {
      
        if (Time.time >= nextDodgeTime)
        {
            
            float randomX = Random.Range(-1f, 1f); 
            float randomZ = Random.Range(-0.8f, 0.2f); 

            Vector3 rightMovement = playerCamera.right * randomX;
            Vector3 forwardMovement = playerCamera.forward * randomZ;

            currentDodgeDirection = (rightMovement + forwardMovement).normalized;
            nextDodgeTime = Time.time + dodgeInterval;
        }

     
        transform.position += currentDodgeDirection * dodgeSpeed * Time.deltaTime;
        

        float newY = playerCamera.position.y + (Mathf.Sin(Time.time * floatSpeed) * floatHeight);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    IEnumerator JumpScareAttack()
    {
        isAttacking = true; 

        Vector3 originalPos = transform.position;
        Vector3 targetPos = playerCamera.position + (playerCamera.forward * 0.5f);

        float dashDuration = 0.1f;
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            transform.position = Vector3.Lerp(originalPos, targetPos, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null; 
        }

        if (targetPlayer != null) 
        {
            targetPlayer.TakeDamage(10f); 
        }

        yield return new WaitForSeconds(0.2f);

        float returnDuration = 0.3f;
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            transform.position = Vector3.Lerp(targetPos, originalPos, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isAttacking = false; 
    }
}