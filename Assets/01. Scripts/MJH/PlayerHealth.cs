using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("전투 (히트스캔 저격)")]
    public float range = 50f;          
    public float attackDamage = 10f;   

    [Header("자원 (탄력/정신력)")]
    public int maxAmmo = 10;             
    private int currentAmmo;             
    public float rechargeTime = 3.0f;    
    private float rechargeTimer = 0f;    
    public TextMeshProUGUI ammoUIText;        

    private Camera arCamera;

    void Start()
    {
        currentHealth = maxHealth;
        currentAmmo = maxAmmo; 
        if (damageFlash != null) damageFlash.color = Color.clear; 
        arCamera = Camera.main;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (currentAmmo < maxAmmo)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer >= rechargeTime)
            {
                currentAmmo++;
                rechargeTimer = 0f;
                UpdateAmmoUI();
            }
        }

        if (isDamaged) damageFlash.color = flashColor;
        else if (damageFlash != null) damageFlash.color = Color.Lerp(damageFlash.color, Color.clear, flashSpeed * Time.deltaTime);
        
        isDamaged = false;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        isDamaged = true; 
        if (currentHealth <= 0) Debug.Log("<color=black>사망했습니다.</color>");
    }

    public void OnAttackButtonPressed()
    {
        if (arCamera == null || currentAmmo <= 0) return;

        currentAmmo--;
        rechargeTimer = 0f; 
        UpdateAmmoUI();

        Ray ray = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            TargetHealth target = hit.transform.GetComponent<TargetHealth>();
            if (target != null)
            {
                // 데미지와 타격 방향 2가지를 적에게 전송
                target.TakeDamage(attackDamage, ray.direction);
            }
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoUIText != null) ammoUIText.text = $"{currentAmmo} / {maxAmmo}";
    }
}