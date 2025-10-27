using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceTree : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;

    // The object (tree) that will be placed in the AR scene.
    [SerializeField] private GameObject treePrefab;

    // flag to prevent multiple placements from a single tap.
    bool isPlacing = false;

    // I store the raycast hit results in this list. It prevents multiple placements from happening instantly when the screen is touched. 
    private static readonly List<ARRaycastHit> hits = new();

    void Update()
    {
        // If core components are not assigned, do nothing.
        if (!raycastManager || !treePrefab) return;

        // Check if the user has touched the screen and the finger just began touching.
        // Also we need to ensure that we are not already in the middle of placing an object.
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !isPlacing)
        {
            isPlacing = true;// Lock further placements temporarily.

            // Get the 2D position of the touch on the screen.
            Vector2 touchPos = Input.GetTouch(0).position;

            // Attempt to place the object at the touched position.
            PlaceObject(touchPos);
        }
    }

    void PlaceObject(Vector2 touchPosition)
    {

        // This will perform a raycast from the touch point into the AR scene by only considering hits on detected planes.
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {

            // Use the first hit result as the location and rotation.
            Pose pose = hits[0].pose;

            // Instantiate the object slightly above the surface so it does not clip into the plane.
            Instantiate(treePrefab, pose.position + Vector3.up * 0.05f, pose.rotation);
        }

        StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        // Small delay to ensure we do not accidentally place multiple objects on one tap.
        yield return new WaitForSeconds(0.2f);
        isPlacing = false;
    }
}


/*
Information about unity setup with this code:

1. We add thi script to XR Session Origin GameObject in the scene.
2. We assign the ARRaycastManager component from the XR Session Origin to the raycast manager field in the inspector.
3. We create a prefab of the tree model we want to place in AR and assign it to the treePrefab field in the inspector.
4. We ensure that ARPlaneManager is also added to the XR Session Origin to detect planes in the environment.
5. We add the tree model prefab to the ARPlaceTree script in the Unity Inspector.
6. The treeprefab will appear when we tap on detected planes in the AR view.
*/
