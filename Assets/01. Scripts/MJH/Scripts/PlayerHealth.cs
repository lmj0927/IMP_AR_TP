using UnityEngine;
using UnityEngine.UI;

using UnityEngine.InputSystem; // 신형 입력 시스템 사용을 위한 필수 선언
using UnityEngine.EventSystems;
public class PlayerHealth : MonoBehaviour
{
    [Header("생존 수치")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("피격 피드백")]
    public Image damageFlash;      
    public float flashSpeed = 5f;          
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); 

    private bool isDamaged = false;         

    public Transform muzzle;
    public float range = 50f;
    public float attackDamage = 10f;
    public GameObject bulletPrefab;
    
    private Camera arCamera;

    void Start()
    {
        currentHealth = maxHealth;
        if (damageFlash != null) damageFlash.color = Color.clear; 
         arCamera = Camera.main;
    }

    void Update()
    {
        
        if (isDamaged)
        {
            damageFlash.color = flashColor;
        }
        else if (damageFlash != null)
        {
            damageFlash.color = Color.Lerp(damageFlash.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        
        isDamaged = false;

        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
      
            //if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) 
               // return;

            FireAtPointer();
    }
    }

   
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        isDamaged = true; 

        Debug.Log($"<color=orange>[플레이어 피격!]</color> 으악! 남은 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("<color=black>당신은 사망했습니다. 게임 오버.</color>");
   
    }
    void FireAtPointer()
    {
      Vector2 screenPosition = Vector2.zero;

      
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            screenPosition = Touchscreen.current.touches[0].position.ReadValue();
        else if (Mouse.current != null)
            screenPosition = Mouse.current.position.ReadValue();
        else return;

   
        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        
        if (bulletPrefab != null && muzzle != null)
        {
            Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(ray.direction));
            Debug.Log("<color=cyan>[투사체 발사!]</color> 탕!");
        }
        else
        {
            Debug.LogError("총알 프리팹이나 총구(Muzzle) 위치가 인스펙터에 할당되지 않았습니다!");
        }
}
}
