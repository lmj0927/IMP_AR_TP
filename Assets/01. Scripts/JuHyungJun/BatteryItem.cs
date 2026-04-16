using System;
using UnityEngine;

// Represents the individual collectable item in the scene.
// Handles the visual hover/rotate effects and collection logic.

public class BatteryItem : MonoBehaviour
{
    [Header("Battery Data")]
    [SerializeField] private float filterTimeAmount = 5f;

    [Header("Floating Animation")]
    [SerializeField] private float moveSpeed = 2f;      // Floating speed
    [SerializeField] private float moveAmount = 0.1f;    // Floating range 10cm
    [SerializeField] private bool rotate = true;        // Enable rotation
    [SerializeField] private float rotateSpeed = 50f;   // Rotation speed

    private Vector3 startPosition;

    public float FilterTimeAmount => filterTimeAmount;
    public Action<BatteryItem> OnCollected; // Event triggered when collected

    private void Start()
    {
        // Store the initial position for the floating animation
        startPosition = transform.position;
    }

    private void Update()
    {
        // vertical hover using a Sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        // rotating battery
        if (rotate)
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
    
    //Handles the collection logic, deactivation, and event broadcasting
    public void Collect()
    {
        if (!gameObject.activeSelf)
            return;
        // Notify the spawner to start the respawn timer
        gameObject.SetActive(false);
        OnCollected?.Invoke(this); // Trigger Spawner to start respawn coroutine
        GameManager.Instance.IncreaseLeftFilterTime(filterTimeAmount);
    }

    // Update startPosition when respawning to prevent Y-axis drifting
    private void OnEnable()
    {
        startPosition = transform.position;
    }
}