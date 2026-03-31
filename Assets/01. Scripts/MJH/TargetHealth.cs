using UnityEngine;

public class TargetHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public float maxHealth = 30f; // 최대 체력 (총 3방 맞으면 죽게 설정)
    private float currentHealth;

    void Start()
    {
        // 게임이 시작되면 현재 체력을 최대 체력으로 채웁니다.
        currentHealth = maxHealth;
    }

    // 외부(플레이어의 총알)에서 데미지를 전달할 때 호출할 공개(public) 함수
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"<color=orange>{gameObject.name} 피격!</color> 남은 체력: {currentHealth}");

        // 체력이 0 이하로 떨어지면 사망 처리
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"<color=red>{gameObject.name} 파괴됨!</color>");
        
        // 여기에 나중에 폭발 이펙트나 점수 증가 로직이 들어갑니다.
        
        // 씬에서 이 오브젝트를 완전히 삭제합니다.
        Destroy(gameObject); 
    }
}