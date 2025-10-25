using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Spawns apple projectiles from detected AR planes at regular intervals.
/// It filters horizontal planes, samples points that lie inside the camera's viewport,
/// and launches apples toward the player so targets always enter the visible play space.
/// </summary>
[RequireComponent(typeof(ARPlaneManager))]
public class AppleLauncher : MonoBehaviour
{
    [SerializeField] private GameObject applePrefab;
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private float spawnDelay = 1.5f;
    [SerializeField] private float spawnHeightOffset = 0.05f;
    [SerializeField] private float minHorizontalSpeed = 1.5f;
    [SerializeField] private float maxHorizontalSpeed = 2.5f;
    [SerializeField] private float verticalSpeed = 3.5f;
    [SerializeField] private float randomSpinDegreesPerSecond = 180f;
    [SerializeField] private Camera targetCamera;
    [SerializeField, Range(0f, 0.45f)] private float viewportPadding = 0.12f;
    [SerializeField] private int maxSpawnAttempts = 6;

    private ARPlaneManager _planeManager;
    private Coroutine _spawnRoutine;
    private readonly List<ARPlane> _candidatePlanes = new();
    private Camera _cachedCamera;

    private void Awake()
    {
        _planeManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        if (_planeManager != null)
        {
            _planeManager.planesChanged += OnPlanesChanged;
        }

        RefreshPlaneCache();
        TryStartSpawner();
    }

    private void OnDisable()
    {
        if (_planeManager != null)
        {
            _planeManager.planesChanged -= OnPlanesChanged;
        }

        StopSpawner();
        _candidatePlanes.Clear();
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        RefreshPlaneCache();
        TryStartSpawner();
    }

    private void RefreshPlaneCache()
    {
        _candidatePlanes.Clear();

        if (_planeManager == null)
        {
            return;
        }

        foreach (var plane in _planeManager.trackables)
        {
            if (!plane.enabled || !plane.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
            {
                continue;
            }

            if (plane.size.x < 0.05f || plane.size.y < 0.05f)
            {
                continue;
            }

            _candidatePlanes.Add(plane);
        }
    }

    private void TryStartSpawner()
    {
        if (!isActiveAndEnabled || applePrefab == null || _candidatePlanes.Count == 0)
        {
            return;
        }

        if (_spawnRoutine == null)
        {
            _spawnRoutine = StartCoroutine(SpawnLoop());
        }
    }

    private void StopSpawner()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(spawnDelay);

        while (true)
        {
            SpawnApple();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnApple()
    {
        if (applePrefab == null || _candidatePlanes.Count == 0)
        {
            return;
        }

        if (!TryGetVisibleSpawnPoint(out var plane, out var spawnPosition))
        {
            return;
        }

        // Orient toward the camera when possible so apples fly through the player’s view frustum.
        Vector3 planeUp = plane.transform.up;
        Vector3 lookDirection = GetHorizontalDirectionTowardsCamera(spawnPosition, planeUp);
        if (lookDirection.sqrMagnitude <= 0f)
        {
            lookDirection = GetRandomInPlaneDirection(plane.transform);
        }

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection, planeUp);
        GameObject appleInstance = Instantiate(applePrefab, spawnPosition, lookRotation);

        if (appleInstance.TryGetComponent<Rigidbody>(out var rigidbody))
        {
            Vector3 horizontalDirection = lookDirection.sqrMagnitude > 0f ? lookDirection : GetRandomInPlaneDirection(plane.transform);
            float horizontalSpeed = Random.Range(minHorizontalSpeed, maxHorizontalSpeed);
            Vector3 velocity = horizontalDirection * horizontalSpeed + planeUp * verticalSpeed;
            rigidbody.linearVelocity = velocity;

            if (randomSpinDegreesPerSecond > 0f)
            {
                Vector3 angularVelocity = Random.onUnitSphere * randomSpinDegreesPerSecond * Mathf.Deg2Rad;
                rigidbody.angularVelocity = angularVelocity;
            }
        }
    }

    private static Vector3 SamplePointOnPlane(ARPlane plane)
    {
        Vector2 extents = plane.size * 0.5f;
        float localX = Random.Range(-extents.x, extents.x);
        float localZ = Random.Range(-extents.y, extents.y);
        Vector3 localPoint = new Vector3(localX, 0f, localZ);
        return plane.transform.TransformPoint(localPoint);
    }

    private static Vector3 GetRandomInPlaneDirection(Transform planeTransform)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 right = planeTransform.right;
        Vector3 forward = Vector3.Cross(planeTransform.up, right).normalized;
        return (Mathf.Cos(angle) * right + Mathf.Sin(angle) * forward).normalized;
    }

    private bool TryGetVisibleSpawnPoint(out ARPlane plane, out Vector3 spawnPosition)
    {
        // Repeatedly samples plane surfaces until we find a point projected inside the camera viewport.
        plane = null;
        spawnPosition = Vector3.zero;

        Camera cam = GetTargetCamera();
        if (cam == null)
        {
            return false;
        }

        if (_candidatePlanes.Count == 0)
        {
            return false;
        }

        int attempts = Mathf.Max(1, maxSpawnAttempts);
        for (int i = 0; i < attempts; i++)
        {
            ARPlane candidatePlane = _candidatePlanes[Random.Range(0, _candidatePlanes.Count)];
            Vector3 candidatePosition = SamplePointOnPlane(candidatePlane) + candidatePlane.transform.up * spawnHeightOffset;
            if (IsWithinViewport(cam, candidatePosition))
            {
                plane = candidatePlane;
                spawnPosition = candidatePosition;
                return true;
            }
        }

        return false;
    }

    private Camera GetTargetCamera()
    {
        if (targetCamera != null)
        {
            return targetCamera;
        }

        if (_cachedCamera == null)
        {
            _cachedCamera = Camera.main;
        }

        return _cachedCamera;
    }

    private bool IsWithinViewport(Camera cam, Vector3 worldPoint)
    {
        Vector3 viewportPoint = cam.WorldToViewportPoint(worldPoint);
        if (viewportPoint.z <= 0f)
        {
            return false;
        }

        float padding = Mathf.Clamp01(viewportPadding);
        return viewportPoint.x >= padding &&
               viewportPoint.x <= 1f - padding &&
               viewportPoint.y >= padding &&
               viewportPoint.y <= 1f - padding;
    }

    private Vector3 GetHorizontalDirectionTowardsCamera(Vector3 spawnPosition, Vector3 planeUp)
    {
        Camera cam = GetTargetCamera();
        if (cam == null)
        {
            return Vector3.zero;
        }

        Vector3 toCamera = cam.transform.position - spawnPosition;
        Vector3 projected = Vector3.ProjectOnPlane(toCamera, planeUp);
        return projected.sqrMagnitude > 0f ? projected.normalized : Vector3.zero;
    }
}
