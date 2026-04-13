using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.InputSystem; // 신형 입력 시스템 사용을 위한 필수 선언
using UnityEngine.EventSystems;
public class PlayerHealth : MonoBehaviour
{
    [Header("피격 피드백")]
    public Image damageFlash;      
    public float flashSpeed = 5f;          
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); 

    private bool isDamaged = false;         

    public Transform muzzle;
    public float range = 50f;
    public float attackDamage = 10f;
    public GameObject bulletPrefab;

    [SerializeField]
    private List<HpBox> hpBoxes = new List<HpBox>();    
    
    private Camera arCamera;
    private bool isDead = false;

    void Start()
    {
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
    }

   
    public void TakeDamage()
    {
        if(isDead)
            return;
        foreach (var hpbox in hpBoxes)
        {
            if (!hpbox.GetIsOn())
                continue;
            hpbox.OffHpBox();
            break;
        }

        var onHpCount = hpBoxes.Count(x => x.GetIsOn());

        if (onHpCount == 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("<color=black>당신은 사망했습니다. 게임 오버.</color>");
        GameManager.Instance.GameOver();
        isDead = true;
    }
    public void FireAtPointer()
    {
    
        if (bulletPrefab != null && muzzle != null)
        {
            Instantiate(bulletPrefab, arCamera.transform.position + arCamera.transform.forward * 0.3f, Quaternion.LookRotation(arCamera.transform.forward));
            Debug.Log("<color=cyan>[투사체 발사!]</color> 탕!");    
        }
        else
        {
            Debug.LogError("총알 프리팹이나 총구(Muzzle) 위치가 인스펙터에 할당되지 않았습니다!");
        }
    }
}
