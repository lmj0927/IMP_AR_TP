using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.EventSystems;

public class PlayerShooting : MonoBehaviour
{
    [Header("사격 설정")]
    public Transform muzzle;
    public float range = 50f;
    public float attackDamage = 10f;
    public GameObject bulletPrefab;
    
    private Camera arCamera;

    void Start()
    {
        arCamera = Camera.main;
    }

    void Update()
    {
  
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
           
            FireAtPointer();
        }
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