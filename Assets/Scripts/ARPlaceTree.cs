using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceTree : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject cubePrefab;   // <--- use your prefab here

    bool isPlacing = false;
    private static readonly List<ARRaycastHit> hits = new();

    void Update()
    {
        if (!raycastManager || !cubePrefab) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !isPlacing)
        {
            isPlacing = true;
            Vector2 touchPos = Input.GetTouch(0).position;
            PlaceObject(touchPos);
        }
    }

    void PlaceObject(Vector2 touchPosition)
    {
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose pose = hits[0].pose;
            Instantiate(cubePrefab, pose.position + Vector3.up * 0.05f, pose.rotation);
        }

        StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.2f);
        isPlacing = false;
    }
}
