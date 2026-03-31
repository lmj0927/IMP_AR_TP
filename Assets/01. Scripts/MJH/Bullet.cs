using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("투사체 스펙")]
    public float speed = 20f;       // 날아가는 속도
    public float damage = 10f;      // 타격 데미지
    public float lifeTime = 3f;     // 3초 뒤 자동 파괴 (메모리 누수 방지)

    void Start()
    {
        // 1. 태어나자마자 자신의 앞쪽(forward) 방향으로 지정된 속도만큼 물리적 힘을 받아 날아갑니다.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        // 2. 허공으로 날아간 총알이 메모리에 영원히 쌓이는 것을 막기 위한 자폭 타이머
        Destroy(gameObject, lifeTime);
    }

    // 3. 누군가와 부딪혔을 때 (Is Trigger가 켜져 있어야 작동함)
    void OnTriggerEnter(Collider other)
    {
        // 부딪힌 대상이 TargetHealth(적) 스크립트를 가지고 있는지 검사합니다.
        TargetHealth target = other.GetComponent<TargetHealth>();
        
        if (target != null)
        {
            // 적이라면 데미지를 입히고
            target.TakeDamage(damage);
            
            // 데미지를 입힌 총알은 즉시 파괴되어 사라집니다.
            Destroy(gameObject);
        }
    }
}