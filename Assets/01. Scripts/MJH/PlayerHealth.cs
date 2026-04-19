using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("피격 피드백")]
    public Image damageFlash;      
    public float flashSpeed = 5f;          
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); 

    private bool isDamaged = false;         

    [Header("사격 (히트스캔) 설정")]
    public float range = 50f;          // 레이저가 닿는 최대 거리
    public float attackDamage = 10f;   // 적에게 입힐 데미지
    public float fireRate = 0.5f;      // 연사 제한 
    private float lastFireTime = 0f;

    [Header("체력 관리")]
    [SerializeField]
    private List<HpBox> hpBoxes = new List<HpBox>();    
    
    private Camera arCamera;
    private bool isDead = false;
    
    [SerializeField] private GameObject hitVFX;

    void Start()
    {
        if (damageFlash != null) damageFlash.color = Color.clear; 
        arCamera = Camera.main;
    }

    void Update()
    {
        if (isDamaged)
        {
            /*if (damageFlash != null)
                damageFlash.color = Color.Lerp(damageFlash.color, flashColor, flashSpeed * Time.deltaTime);*/
            isDamaged = false;
        }
    }

    // [치명적 결함 수정] EnemyAI가 데미지를 던질 수 있도록 매개변수(float amount)를 뚫어놓았습니다.
    public void TakeDamage()
    {
        if(isDead) return;
        
        isDamaged = true;
        Debug.Log($"<color=red>[피격]</color> 플레이어 피격! 체력이 깎입니다.");

        foreach (var hpbox in hpBoxes)
        {
            if (!hpbox.GetIsOn()) continue;
            hpbox.OffHpBox();
            StartCoroutine(FlashFlash());
            break;
        }

        var onHpCount = hpBoxes.Count(x => x.GetIsOn());

        if (onHpCount == 0)
        {
            Die();
        }
    }
    
    IEnumerator FlashFlash()
    {
        damageFlash.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        damageFlash.color = Color.clear;
    }

    void Die()
    {
        Debug.Log("<color=black>당신은 사망했습니다. 게임 오버.</color>");
        
    
        if (GameManager.Instance != null) GameManager.Instance.GameOver(); 
        
        isDead = true;
    }

   

    public void OnAttackButtonPressed()
    {
        if (isDead) return;

        // 쿨다운 검사: 연속 사격 방지
        if (Time.time >= lastFireTime + fireRate)
        {
            ExecuteHitscan();
            lastFireTime = Time.time;
        }
        else
        {
            Debug.Log("<color=yellow>[재장전 중]</color> 너무 빨리 쏠 수 없습니다.");
        }
    }

    private void ExecuteHitscan()
    {
        if (arCamera == null) return;

       
        Ray ray = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Debug.Log("<color=cyan>[사격]</color> 광선 발사!");

       
        if (Physics.Raycast(ray, out hit, range))
        {
            
            // Fix by Minjun
            if (hit.collider.TryGetComponent<TargetHealth>(out var enemyHealth))
            {
                enemyHealth.TakeDamage(attackDamage, ray.direction);
                Instantiate(hitVFX, hit.point, Quaternion.identity);
            }

            if (hit.collider.TryGetComponent<TargetHealth_Blinker>(out var enemyBlinkerHealth))
            {
                enemyBlinkerHealth.TakeDamage(attackDamage, ray.direction);
                Instantiate(hitVFX, hit.point, Quaternion.identity);
            }
           
            //hit.collider.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
            //hit.collider.SendMessage("ApplyKnockback", ray.direction, SendMessageOptions.DontRequireReceiver);
            
            Debug.Log($"<color=orange>[명중]</color> {hit.collider.name} 타격 성공!");
        }
    }
}