using UnityEngine;

public class TargetHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public float maxHealth = 30f; 
    private float currentHealth;
    
    private EnemyAI enemyAI;

    void Start()
    {
        currentHealth = maxHealth;
        enemyAI = GetComponent<EnemyAI>(); 
    }

    public void TakeDamage(float amount, Vector3 hitDirection)
    {
        currentHealth -= amount;
        Debug.Log(currentHealth);
        
        if (enemyAI != null)
        {
            enemyAI.ApplyKnockback(hitDirection);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        AudioManager.Instance.PlaySound(SoundType.EnemyDie);
        GameManager.Instance.IncreaseStageLevel();
        Destroy(gameObject); 
    }
}