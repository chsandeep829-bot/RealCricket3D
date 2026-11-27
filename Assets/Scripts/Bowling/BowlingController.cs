using UnityEngine;

public class BowlingController : MonoBehaviour
{
    [Header("Delivery")]
    [SerializeField] private CricketBall ballPrefab;
    [SerializeField] private Transform releasePoint;
    [SerializeField, Min(1f)] private float deliverySpeed = 25f;
    [SerializeField] private Vector3 deliveryDirection = Vector3.forward;

    private void Update()
    {
        // Editor prototype input. Replace with UI controls for mobile builds.
        if (Input.GetKeyDown(KeyCode.B))
            BowlDelivery();
    }

    public void BowlDelivery()
    {
        if (ballPrefab == null || releasePoint == null)
        {
            Debug.LogWarning("Assign Ball Prefab and Release Point on BowlingController.");
            return;
        }

        CricketBall ball = Instantiate(ballPrefab, releasePoint.position, releasePoint.rotation);
        Vector3 direction = releasePoint.TransformDirection(deliveryDirection.normalized);
        ball.Bowl(direction * deliverySpeed);
    }

    public void SetDeliverySpeed(float speed)
    {
        deliverySpeed = Mathf.Max(1f, speed);
    }
}
