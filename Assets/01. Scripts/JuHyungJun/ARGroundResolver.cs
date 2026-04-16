using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// Utility script that converts screen-space coordinates into AR plane world-space coordinates.
// Essential for placing objects on detected physical surfaces.

public class ARGroundResolver : MonoBehaviour
{
    [Header("AR Reference")]
    [SerializeField] private ARRaycastManager arRaycastManager;

    [Header("Screen Sample Point")]
    [Range(0f, 1f)] // Screen Center X
    [SerializeField] private float sampleX = 0.5f;

    [Range(0f, 1f)] // Screen Lower-Center Y
    [SerializeField] private float sampleY = 0.35f;

    private static readonly List<ARRaycastHit> hits = new();

    // Performs an AR Raycast to find the nearest physical plane at the specified screen point
    public bool TryResolveGround(out Vector3 groundPosition)
    {
        // Sampling a specific screen point for ground detection
        Vector2 screenPoint = new(Screen.width * sampleX, Screen.height * sampleY);

        //Filter for "PlaneWithinPolygon" to ensure stable placement on recognized surfaces
        if (arRaycastManager.Raycast(screenPoint, hits, TrackableType.PlaneWithinPolygon))
        {
            groundPosition = hits[0].pose.position;
            return true;
        }

        groundPosition = Vector3.zero;
        return false;
    }
}