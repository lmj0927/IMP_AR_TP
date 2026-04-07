using System;
using UnityEngine;

public class BatteryItem : MonoBehaviour
{
    [Header("Battery Data")]
    [SerializeField] private float filterTimeAmount = 5f;

    public float FilterTimeAmount => filterTimeAmount;

    public Action<BatteryItem> OnCollected;

    public void Collect()
    {
        if (!gameObject.activeSelf)
            return;

        gameObject.SetActive(false);
        OnCollected?.Invoke(this);
    }
}