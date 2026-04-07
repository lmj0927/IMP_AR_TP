using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARGroundResolver : MonoBehaviour
{
    [Header("AR Reference")]
    [SerializeField] private ARRaycastManager arRaycastManager;

    [Header("Screen Sample Point")]
    [Range(0f, 1f)]
    [SerializeField] private float sampleX = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] private float sampleY = 0.35f;

    private static readonly List<ARRaycastHit> hits = new();

    public bool TryResolveGround(out Vector3 groundPosition)
    {
        Vector2 screenPoint = new(
            Screen.width * sampleX,
            Screen.height * sampleY
        );

        if (arRaycastManager.Raycast(screenPoint, hits, TrackableType.PlaneWithinPolygon))
        {
            groundPosition = hits[0].pose.position;
            return true;
        }

        groundPosition = Vector3.zero;
        return false;
    }
}