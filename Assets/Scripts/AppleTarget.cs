using UnityEngine;

/// <summary>
/// Handles dart collisions, slices the apple into halves, and triggers simple aim-assist colliders.
/// The apple listens for physical hits or trigger brushes from darts, replaces itself with two halves,
/// and applies impulses so the pieces peel away in mid-air.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class AppleTarget : MonoBehaviour
{
    [SerializeField] private GameObject leftHalfPrefab;
    [SerializeField] private GameObject rightHalfPrefab;
    [SerializeField] private float splitImpulse = 2.5f;
    [SerializeField] private float upwardImpulseFraction = 0.35f;
    [SerializeField] private float halvesLifetime = 6f;
    [SerializeField] private bool useAssistTrigger = true;
    [SerializeField] private float assistTriggerRadius = 0.2f;

    private Rigidbody _rigidbody;
    private bool _isSliced;
    private SphereCollider _assistTrigger;

    public bool IsSliced => _isSliced;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        CreateAssistTrigger();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isSliced)
        {
            return;
        }

        // Physical hits from a rigidbody dart take priority over the trigger-based assist.
        if (collision.rigidbody != null && collision.rigidbody.TryGetComponent(out DartProjectile _))
        {
            Vector3 hitPoint = collision.GetContactPointOrDefault(transform.position);
            Vector3 incomingDirection = collision.relativeVelocity.sqrMagnitude > 0.01f
                ? collision.relativeVelocity.normalized
                : collision.rigidbody.linearVelocity.normalized;

            SliceApple(hitPoint, incomingDirection);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isSliced || !useAssistTrigger || other == null)
        {
            return;
        }

        // Allow near-miss glances detected by the helper trigger to still count as a slice.
        if (other.TryGetComponent<DartProjectile>(out _))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 incomingDirection = other.attachedRigidbody != null && other.attachedRigidbody.linearVelocity.sqrMagnitude > 0.01f
                ? other.attachedRigidbody.linearVelocity.normalized
                : transform.forward;

            SliceApple(hitPoint, incomingDirection);
        }
    }

    private void SliceApple(Vector3 sliceOrigin, Vector3 incomingDirection)
    {
        if (_isSliced)
        {
            return;
        }
        
        //Aarav Sound
        SoundManager.Instance?.PlayHit();
        

        _isSliced = true;

        if (incomingDirection.sqrMagnitude < 0.0001f)
        {
            incomingDirection = _rigidbody != null && _rigidbody.linearVelocity.sqrMagnitude > 0.01f
                ? _rigidbody.linearVelocity.normalized
                : transform.forward;
        }

        // Replace the original apple with two halves propelled away from the impact plane.
        SpawnHalf(leftHalfPrefab, -transform.right, incomingDirection, sliceOrigin);
        SpawnHalf(rightHalfPrefab, transform.right, incomingDirection, sliceOrigin);

        Destroy(gameObject);
    }

    private void SpawnHalf(GameObject prefab, Vector3 lateralDirection, Vector3 travelDirection, Vector3 sliceOrigin)
    {
        if (prefab == null)
        {
            return;
        }

        GameObject half = Instantiate(prefab, sliceOrigin, transform.rotation);

        if (half.TryGetComponent<Rigidbody>(out var halfBody))
        {
            if (_rigidbody != null)
            {
                halfBody.linearVelocity = _rigidbody.linearVelocity;
            }

            // Kick each half slightly sideways and upward to create a satisfying split motion.
            Vector3 launchDirection = (lateralDirection + travelDirection + Vector3.up * upwardImpulseFraction).normalized;
            halfBody.AddForce(launchDirection * splitImpulse, ForceMode.Impulse);
        }

        if (halvesLifetime > 0f)
        {
            Destroy(half, halvesLifetime);
        }
    }

    public Vector3 GetAimPoint(Vector3 fromPosition)
    {
        // Currently returns the apple's centre; extend here if you want off-centre aim points.
        return transform.position;
    }

    private void CreateAssistTrigger()
    {
        if (!useAssistTrigger || assistTriggerRadius <= 0f)
        {
            return;
        }

        // Lightweight trigger enlarges the effective hit area so near-misses still slice the apple.
        _assistTrigger = gameObject.AddComponent<SphereCollider>();
        _assistTrigger.isTrigger = true;
        _assistTrigger.radius = assistTriggerRadius;
        _assistTrigger.center = Vector3.zero;
    }
}

internal static class CollisionExtensions
{
    public static Vector3 GetContactPointOrDefault(this Collision collision, Vector3 fallback)
    {
        return collision.contactCount > 0 ? collision.GetContact(0).point : fallback;
    }
}
