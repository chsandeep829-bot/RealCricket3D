using UnityEngine;
using UnityEngine.Events;

public class FielderAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0.1f)] private float moveSpeed = 7f;
    [SerializeField, Min(0.1f)] private float turnSpeed = 8f;
    [SerializeField, Min(0.1f)] private float pickupDistance = 1.2f;
    [SerializeField] private float groundHeight = 0f;

    [Header("Events")]
    [SerializeField] private UnityEvent onBallCollected;

    private CricketBall targetBall;
    private bool collected;\n    private ProceduralCricketPlayerMotion playerMotion;

    public bool IsBusy => targetBall != null && !collected;

    public void Chase(CricketBall ball)
    {
        targetBall = ball;
        collected = false;
    }

    public void StopChasing()
    {
        targetBall = null;
        collected = false;
    }

    private void Update()
    {
        if (targetBall == null || collected)\n        {\n            if (playerMotion != null) playerMotion.SetMovementSpeed(0f);\n            return;\n        }

        Vector3 target = targetBall.transform.position;
        target.y = groundHeight;

        Vector3 flatPosition = transform.position;
        flatPosition.y = groundHeight;

        Vector3 delta = target - flatPosition;
        if (delta.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(delta.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        if (delta.magnitude > pickupDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, target, moveSpeed * Time.deltaTime);
            return;
        }

        collected = true;
        onBallCollected?.Invoke();
    }
}
