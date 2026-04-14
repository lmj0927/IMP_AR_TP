using System;
using UnityEngine;

public class BatteryItem : MonoBehaviour
{
    [Header("Battery Data")]
    [SerializeField] private float filterTimeAmount = 5f;

    [Header("Floating Animation")]
    [SerializeField] private float moveSpeed = 2f;      // 움직이는 속도
    [SerializeField] private float moveAmount = 0.1f;    // 움직이는 범위 (0.1m = 10cm)
    [SerializeField] private bool rotate = true;        // 회전 여부
    [SerializeField] private float rotateSpeed = 50f;   // 회전 속도

    private Vector3 startPosition;

    public float FilterTimeAmount => filterTimeAmount;
    public Action<BatteryItem> OnCollected;

    private void Start()
    {
        // 시작 위치를 저장해둡니다.
        startPosition = transform.position;
    }

    private void Update()
    {
        // Sin 곡선으로 위 아래로 왔다갔다
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // 2. 회전 효과 (선택 사항)
        if (rotate)
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }

    public void Collect()
    {
        if (!gameObject.activeSelf)
            return;

        gameObject.SetActive(false);
        OnCollected?.Invoke(this);
        GameManager.Instance.IncreaseLeftFilterTime(filterTimeAmount);
    }

    // 리스폰 될 때 위치 초기화를 위해 OnEnable 사용
    private void OnEnable()
    {
        startPosition = transform.position;
    }
}