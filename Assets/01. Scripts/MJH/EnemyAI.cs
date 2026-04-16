using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("거리 경계선")]
    public float detectionRange = 5.0f;  
    public float attackRange = 1.5f;     
    public float wanderRadius = 4.0f;    
    
    [Header("속도 및 특성")]
    public float wanderSpeed = 0.8f;   
    public float chaseSpeed = 2.0f;      
    public float floatSpeed = 2.0f;      
    public float floatHeight = 0.3f;     
    public float attackCooldown = 2.0f;

    [Header("회피 및 넉백 설정")]
    public float dodgeSpeed = 2.5f;       
    public float dodgeInterval = 0.5f;   
    public float knockbackForce = 1.0f;     
    public float staggerDuration = 0.5f;    
    public float awakeDelay = 3.0f;      

    private float spawnTime;             
    private bool isAwake = false;        
    private bool isStaggered = false;    

    private Vector3 currentDodgeDirection;
    private float nextDodgeTime;
    private Transform playerCamera;
    private PlayerHealth targetPlayer;
    private Vector3 targetWanderPoint;
    private float lastAttackTime;
    private bool isAttacking = false;

    void Start()
    {
        if (Camera.main != null) 
        {
            playerCamera = Camera.main.transform;
            targetPlayer = playerCamera.GetComponent<PlayerHealth>(); 
        }
        SetNewWanderPoint();
        spawnTime = Time.time; 
    }

    void Update()
    {
        if (playerCamera == null || isAttacking || isStaggered) return;

        if (!isAwake)
        {
            transform.LookAt(playerCamera); 
            ApplyFloatingEffect();          
            if (Time.time >= spawnTime + awakeDelay) isAwake = true;
            return; 
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerCamera.position);

        if (distanceToPlayer > detectionRange) WanderAround(); 
        else if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange) ChasePlayer(); 
        else EngagePlayer(); 
    }

    public void ApplyKnockback(Vector3 hitDirection)
    {
        if (!isAwake || isAttacking) return; 
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

    void WanderAround()
    {
        Vector3 direction = (targetWanderPoint - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
        }
        transform.position = Vector3.MoveTowards(transform.position, targetWanderPoint, wanderSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetWanderPoint) < 0.2f) SetNewWanderPoint();
        ApplyFloatingEffect();
    }

    void SetNewWanderPoint()
    {
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
        targetWanderPoint = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);
        targetWanderPoint.y = transform.position.y; 
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
            StartCoroutine(JumpScareAttack());
            lastAttackTime = Time.time;
        }
        else DodgeMovement();
    }

    void DodgeMovement()
    {
        if (Time.time >= nextDodgeTime)
        {
            float randomX = Random.Range(-3f, 3f); 
            Vector3 rightMovement = playerCamera.right * randomX;
            currentDodgeDirection = rightMovement.normalized;
            nextDodgeTime = Time.time + dodgeInterval;
        }
        transform.position += currentDodgeDirection * dodgeSpeed * Time.deltaTime;
        ApplyFloatingEffect(); 
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

        if (targetPlayer != null) targetPlayer.TakeDamage(10f); 
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