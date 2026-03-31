using UnityEngine;
using UnityEngine.InputSystem; // 신형 입력 시스템 사용을 위한 필수 선언
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
        // Pointer.current는 현재 연결된 터치스크린이나 마우스를 자동으로 감지합니다.
        // wasPressedThisFrame은 "방금 화면이 눌렸는가?"를 확인하는 신형 문법입니다.
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            // UI를 터치했을 때는 총이 나가지 않도록 방어
            //if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) 
               // return;

            FireAtPointer();
        }
    }

   void FireAtPointer()
    {
      Vector2 screenPosition = Vector2.zero;

        // 1. 터치/마우스 좌표 획득 (이전과 동일)
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            screenPosition = Touchscreen.current.touches[0].position.ReadValue();
        else if (Mouse.current != null)
            screenPosition = Mouse.current.position.ReadValue();
        else return;

        // 2. 터치한 방향을 향하는 방향 벡터(Ray) 계산
        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        // 3. [핵심] 레이캐스트를 쏘는 대신, 총구(muzzle) 위치에 총알(Prefab)을 실제로 생성합니다.
        // 바라보는 방향(Rotation)은 터치한 곳을 향하는 광선의 방향(ray.direction)과 동일하게 맞춥니다.
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