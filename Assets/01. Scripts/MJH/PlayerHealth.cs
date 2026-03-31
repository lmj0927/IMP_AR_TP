using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("생존 수치")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("피격 피드백")]
    public Image damageFlash;               // 아까 만든 붉은 화면 UI
    public float flashSpeed = 5f;           // 붉은색이 사라지는 속도
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); // 맞았을 때의 반투명 빨간색

    private bool isDamaged = false;         // 이번 프레임에 맞았는지 판별

    void Start()
    {
        currentHealth = maxHealth;
        if (damageFlash != null) damageFlash.color = Color.clear; // 시작 시 투명화 보장
    }

    void Update()
    {
        // 맞은 순간에는 화면을 붉게 만들고, 안 맞고 있을 때는 서서히 다시 투명하게 뺍니다.
        if (isDamaged)
        {
            damageFlash.color = flashColor;
        }
        else if (damageFlash != null)
        {
            damageFlash.color = Color.Lerp(damageFlash.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        
        isDamaged = false; // 매 프레임 상태 리셋
    }

    // 적이 플레이어를 때릴 때 호출할 함수
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        isDamaged = true; // 피격 플래그 온

        Debug.Log($"<color=orange>[플레이어 피격!]</color> 으악! 남은 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("<color=black>당신은 사망했습니다. 게임 오버.</color>");
        // 차후 여기에 게임 오버 씬으로 넘어가는 로직을 추가할 겁니다.
    }
}
