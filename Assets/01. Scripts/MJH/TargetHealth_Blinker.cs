using UnityEngine;

public class TargetHealth_Blinker : MonoBehaviour
{
    [Header("체력 설정 (점멸자)")]
    public float maxHealth = 20f; // 일반 적보다 체력이 낮아 2방에 죽게 세팅 
    private float currentHealth;
    
    private EnemyAI_Blinker blinkerAI;

    void Start()
    {
        currentHealth = maxHealth;
        blinkerAI = GetComponent<EnemyAI_Blinker>(); 
    }

    // PlayerHealth.cs에서 던지는 2개의 인자(데미지, 방향)를 받는 규격은 동일하게 유지
    public void TakeDamage(float amount, Vector3 hitDirection)
    {
        currentHealth -= amount;
        Debug.Log($"<color=orange>[타격]</color> 점멸자에게 데미지! 남은 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (blinkerAI != null)
        {
            // 죽지 않았다면 맞은 즉시 텔레포트하여 숨어버림
            blinkerAI.BlinkAwayOnHit();
        }
    }

    void Die()
    {
        AudioManager.Instance.PlaySound(SoundType.EnemyDie);
        GameManager.Instance.IncreaseStageLevel();
        Debug.Log("<color=black>[점멸자 소멸]</color> 적을 처치했습니다!");
        Destroy(gameObject); 
    }
}