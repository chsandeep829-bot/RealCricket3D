using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CricketBall : MonoBehaviour
{
    [SerializeField] private Rigidbody body;
    [SerializeField] private float maxLifetime = 12f;

    private float spawnedAt;
    private bool hasBeenHit;
    private bool dismissalRecorded;
    private bool deliveryResolved;

    public bool HasBeenHit => hasBeenHit;
    public bool DismissalRecorded => dismissalRecorded;
    public bool DeliveryResolved => deliveryResolved;
    public float SpawnedAt => spawnedAt;
    public Rigidbody Body => body;

    private void Awake()
    {
        if (body == null) body = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        spawnedAt = Time.time;
        hasBeenHit = false;
        dismissalRecorded = false;
        deliveryResolved = false;
    }

    private void Update()
    {
        if (maxLifetime > 0f && Time.time - spawnedAt > maxLifetime)
            Destroy(gameObject);
    }

    public void Bowl(Vector3 velocity)
    {
        hasBeenHit = false;
        dismissalRecorded = false;
        deliveryResolved = false;
        body.isKinematic = false;
        body.velocity = velocity;
    }

    public void Hit(Vector3 velocity)
    {
        hasBeenHit = true;
        dismissalRecorded = false;
        deliveryResolved = false;
        body.isKinematic = false;
        body.velocity = velocity;
    }

    public void ApplyFieldingThrow(Vector3 velocity)
    {
        if (body == null || !hasBeenHit || dismissalRecorded || deliveryResolved) return;
        body.isKinematic = false;
        body.velocity = velocity;
    }

    public void ResetBall(Vector3 position, Quaternion rotation)
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.isKinematic = true;
        transform.SetPositionAndRotation(position, rotation);
        hasBeenHit = false;
        dismissalRecorded = false;
        deliveryResolved = false;
        spawnedAt = Time.time;
    }

    public void MarkDismissal()
    {
        dismissalRecorded = true;
        deliveryResolved = true;
    }

    public bool MarkDeliveryResolved()
    {
        if (deliveryResolved || dismissalRecorded) return false;
        deliveryResolved = true;
        return true;
    }
}