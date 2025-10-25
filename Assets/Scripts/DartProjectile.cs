using UnityEngine;

/// <summary>
/// Handles per-dart behaviours such as aim assist steering and exposes configuration hooks.
/// Aim assist gradually rotates the velocity vector toward a nearby AppleTarget within range so
/// the dart subtly curves rather than snapping, keeping the throw feeling natural.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DartProjectile : MonoBehaviour
{
    [Header("Aim Assist")]
    [SerializeField] private bool aimAssistEnabled = false;
    [SerializeField] private float assistRadius = 0.75f;
    [SerializeField] private float turnRateDegreesPerSecond = 120f;
    [SerializeField] private float maxAngleDegrees = 35f;
    [SerializeField] private LayerMask targetLayers = ~0;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void ConfigureAimAssist(bool enabled, float radius, float turnRateDegrees, float maxAngle, LayerMask layers)
    {
        // Called by ThrowDarts when the projectile is spawned so we can mirror the latest inspector tuning.
        aimAssistEnabled = enabled;
        assistRadius = radius;
        turnRateDegreesPerSecond = turnRateDegrees;
        maxAngleDegrees = maxAngle;
        targetLayers = layers;
    }

    private void FixedUpdate()
    {
        if (!aimAssistEnabled || _rigidbody == null || assistRadius <= 0f)
        {
            return;
        }

        Vector3 velocity = _rigidbody.linearVelocity;
        float speed = velocity.magnitude;
        if (speed < 0.05f)
        {
            return;
        }

        Vector3 currentDirection = velocity / speed;
        AppleTarget target = FindBestTarget(currentDirection);
        if (target == null)
        {
            return;
        }

        // Steer toward the apple in small increments so the trajectory bends smoothly.
        Vector3 desiredDirection = (target.GetAimPoint(transform.position) - transform.position).normalized;
        if (desiredDirection.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float maxRadians = Mathf.Deg2Rad * Mathf.Max(0f, turnRateDegreesPerSecond) * Time.fixedDeltaTime;
        Vector3 newDirection = Vector3.RotateTowards(currentDirection, desiredDirection, maxRadians, 0f);
        _rigidbody.linearVelocity = newDirection * speed;
    }

    private AppleTarget FindBestTarget(Vector3 velocityDirection)
    {
        // Prefer apples that are close and roughly ahead of the dart to avoid hard 90° turns.
        Collider[] hits = Physics.OverlapSphere(transform.position, assistRadius, targetLayers, QueryTriggerInteraction.Collide);
        AppleTarget best = null;
        float bestScore = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            AppleTarget candidate = hit.GetComponentInParent<AppleTarget>();
            if (candidate == null || candidate.IsSliced)
            {
                continue;
            }

            Vector3 toTarget = candidate.GetAimPoint(transform.position) - transform.position;
            float distance = toTarget.magnitude;
            if (distance <= Mathf.Epsilon)
            {
                continue;
            }

            float angle = Vector3.Angle(velocityDirection, toTarget);
            if (angle > maxAngleDegrees)
            {
                continue;
            }

            float score = angle + distance;
            if (score < bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }
}
