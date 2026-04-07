using UnityEngine;
using System.Collections;
public class EnemyAI : MonoBehaviour
{
    [Header("상태 및 거리 설정")]
    public float attackRange = 2.5f;    
    public float wanderRadius = 4.0f;   
    [Header("원혼 이동 특성 (핵심 수술 부위)")]
    public float moveSpeed = 1.0f;      
    public float floatSpeed = 2.0f;      
    public float floatHeight = 0.3f;     
    public float attackCooldown = 2.0f;

    private Transform playerCamera;
    private Vector3 targetWanderPoint;
    private float lastAttackTime;
    

    private float originalY;
    private PlayerHealth targetPlayer;
    private bool isAttacking = false;

    void Start()
    {
        if (Camera.main != null) playerCamera = Camera.main.transform;
       
        SetNewWanderPoint();

    targetPlayer = playerCamera.GetComponent<PlayerHealth>();
    
    }

    void Update()
    {
        if (playerCamera == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerCamera.position);

        if (distanceToPlayer > attackRange)
        {
            WanderAround();
        }
        else
        {
            AttackPlayer();
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

        transform.position = Vector3.MoveTowards(transform.position, targetWanderPoint, moveSpeed * Time.deltaTime);
        

        if (Vector3.Distance(transform.position, targetWanderPoint) < 0.2f)
        {
            SetNewWanderPoint();
        }
     ApplyFloatingEffect();
    }

    void SetNewWanderPoint()
    {
        
        Vector3 randomPoint = Random.insideUnitSphere * wanderRadius;
        targetWanderPoint = playerCamera.position + randomPoint;

      
        if (targetWanderPoint.y < playerCamera.position.y - 1.0f)
        {
            targetWanderPoint.y = playerCamera.position.y;
        }

        originalY = targetWanderPoint.y;
    }

    void ApplyFloatingEffect()
    {
       
        float newY = transform.position.y + (Mathf.Sin(Time.time * floatSpeed) * floatHeight * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void AttackPlayer()
    {
        
        transform.LookAt(playerCamera);

        
        ApplyFloatingEffect();

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log($"<color=magenta>[심령 공격!]</color> 원혼이 당신을 덮칩니다! (거리: {Vector3.Distance(transform.position, playerCamera.position):F1}m)");
            
           
            StartCoroutine(JumpScareAttack());
            
            lastAttackTime = Time.time;
        }
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
        else
        {
            Debug.LogWarning("치명적 에러: 타겟에게 PlayerHealth 스크립트가 없습니다!");
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