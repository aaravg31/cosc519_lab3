using UnityEngine;

/// <summary>
/// Detects an upward swipe from the bottom of the screen and throws a dart prefab forward.
/// The script samples the first touch, ensures it begins near the bottom edge, computes the swipe direction,
/// instantiates the dart, and configures spin/aim-assist before applying an initial velocity.
/// </summary>
public class ThrowDarts : MonoBehaviour
{
    [Header("Dart")]
    [SerializeField] private GameObject dartPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float baseThrowSpeed = 6f;
    [SerializeField] private float swipeSpeedMultiplier = 0.0025f;
    [SerializeField] private float maxThrowSpeed = 15f;
    [SerializeField] private float verticalArcAngle = 10f;
    [SerializeField] private float spinSpeedDegreesPerSecond = 720f;
    [SerializeField] private Vector3 localSpinAxis = Vector3.up;
    [SerializeField] private bool spinClockwise = true;
    [SerializeField] private float dartLifetime = 10f;

    [Header("Aim Assist")]
    // Tuneable parameters forwarded to DartProjectile so each dart can bend gently toward nearby apples.
    [SerializeField] private bool enableAimAssist = true;
    [SerializeField] private float aimAssistRadius = 0.75f;
    [SerializeField] private float aimAssistTurnRateDegrees = 120f;
    [SerializeField] private float aimAssistMaxAngleDegrees = 35f;
    [SerializeField] private LayerMask aimAssistLayerMask = ~0;

    [Header("Swipe Detection")]
    [SerializeField] private float minSwipeDistance = 150f;
    [SerializeField] private float maxSwipeDuration = 0.75f;
    [SerializeField, Range(0f, 0.5f)] private float bottomScreenPercent = 0.18f;

    private Vector2 _touchStartPosition;
    private float _touchStartTime;
    private bool _swipeStartedNearBottom;

    private void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                HandleTouchBegan(touch);
                break;
            case TouchPhase.Ended:
                HandleTouchEnded(touch);
                break;
            case TouchPhase.Canceled:
                ResetSwipeState();
                break;
        }
    }

    private void HandleTouchBegan(Touch touch)
    {
        _touchStartPosition = touch.position;
        _touchStartTime = Time.time;
        _swipeStartedNearBottom = TouchStartedNearBottom(_touchStartPosition);
    }

    private void HandleTouchEnded(Touch touch)
    {
        if (!_swipeStartedNearBottom || dartPrefab == null)
        {
            ResetSwipeState();
            return;
        }

        Vector2 swipeDelta = touch.position - _touchStartPosition;
        float swipeDuration = Time.time - _touchStartTime;

        bool isUpwardSwipe = swipeDelta.y > 0f;
        bool meetsDistance = swipeDelta.magnitude >= minSwipeDistance;
        bool meetsDuration = swipeDuration <= maxSwipeDuration;

        if (isUpwardSwipe && meetsDistance && meetsDuration)
        {
            ThrowDart(swipeDelta, swipeDuration);
        }

        ResetSwipeState();
    }

    private void ThrowDart(Vector2 swipeDelta, float swipeDuration)
    {
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        GameObject dartInstance = Instantiate(dartPrefab, spawnPosition, spawnRotation);
        
        //Aarav Sound
        SoundManager.Instance?.PlayThrow();
        
        SetupDartComponents(dartInstance);

        Rigidbody dartBody = dartInstance.GetComponent<Rigidbody>();
        if (dartBody == null)
        {
            Debug.LogWarning("Dart prefab needs a Rigidbody component to be thrown.", dartInstance);
            return;
        }

        // Convert swipe magnitude and duration into a clamped launch speed to keep throws responsive but controlled.
        float swipeSpeed = swipeDelta.magnitude / Mathf.Max(swipeDuration, 0.05f);
        float throwSpeed = Mathf.Clamp(baseThrowSpeed + swipeSpeed * swipeSpeedMultiplier, baseThrowSpeed, maxThrowSpeed);

        Vector3 throwDirection = spawnPoint != null ? spawnPoint.forward : transform.forward;
        if (verticalArcAngle != 0f)
        {
            throwDirection = Quaternion.AngleAxis(verticalArcAngle, transform.right) * throwDirection;
        }

        Vector3 normalizedDirection = throwDirection.normalized;
        dartBody.linearVelocity = normalizedDirection * throwSpeed;
        Vector3 spinAxis = ResolveSpinAxis();
        float spinSign = spinClockwise ? -1f : 1f;
        dartBody.angularVelocity = spinAxis * spinSpeedDegreesPerSecond * Mathf.Deg2Rad * spinSign;

        if (dartLifetime > 0f)
        {
            Destroy(dartInstance, dartLifetime);
        }
    }

    private bool TouchStartedNearBottom(Vector2 touchPosition)
    {
        float bottomThreshold = Screen.height * bottomScreenPercent;
        return touchPosition.y <= bottomThreshold;
    }

    private void ResetSwipeState()
    {
        _swipeStartedNearBottom = false;
        _touchStartTime = 0f;
        _touchStartPosition = Vector2.zero;
    }

    private void SetupDartComponents(GameObject dartInstance)
    {
        // Ensures every spawned dart has the behaviour script and pushes the current aim-assist settings to it.
        if (!dartInstance.TryGetComponent<DartProjectile>(out var projectile))
        {
            projectile = dartInstance.AddComponent<DartProjectile>();
        }

        projectile.ConfigureAimAssist(enableAimAssist, aimAssistRadius, aimAssistTurnRateDegrees, aimAssistMaxAngleDegrees, aimAssistLayerMask);
    }

    private Vector3 ResolveSpinAxis()
    {
        Transform axisSource = spawnPoint != null ? spawnPoint : transform;
        Vector3 worldAxis = axisSource.TransformDirection(localSpinAxis);
        if (worldAxis.sqrMagnitude < 0.0001f)
        {
            return axisSource.forward;
        }

        // Normalise to avoid scaling angular velocity by the transform scale.
        return worldAxis.normalized;
    }
}
