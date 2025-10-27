using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class TapToPlaceTree : MonoBehaviour
{
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private ARRaycastManager raycastManager;

    private List<ARRaycastHit> hits = new();

    private void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        // Only act on touch begin
        if (touch.phase == TouchPhase.Began)
        {
            if (raycastManager == null)
                return;

            // Raycast against detected planes
            if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes))
            {
                Pose hitPose = hits[0].pose;

                // Spawn tree prefab at hit point
                Instantiate(treePrefab, hitPose.position, hitPose.rotation);
            }
        }
    }
}