using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CricketBall : MonoBehaviour
{
    [SerializeField] private Rigidbody body;
    [SerializeField] private float maxLifetime = 12f;

    private float spawnedAt;
    private bool hasBeenHit;

    public bool HasBeenHit => hasBeenHit;
    public Rigidbody Body => body;

    private void Awake()
    {
        if (body == null) body = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        spawnedAt = Time.time;
        hasBeenHit = false;
    }

    private void Update()
    {
        if (maxLifetime > 0f && Time.time - spawnedAt > maxLifetime)
            Destroy(gameObject);
    }

    public void Bowl(Vector3 velocity)
    {
        hasBeenHit = false;
        body.isKinematic = false;
        body.velocity = velocity;
    }

    public void Hit(Vector3 velocity)
    {
        hasBeenHit = true;
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
        spawnedAt = Time.time;
    }
}
