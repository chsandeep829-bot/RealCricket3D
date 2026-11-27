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
    private bool collected;
    private ProceduralCricketPlayerMotion playerMotion;

    public bool IsBusy => targetBall != null && !collected;

    private void Awake()
    {
        playerMotion = GetComponent<ProceduralCricketPlayerMotion>();
    }

    public void Chase(CricketBall ball)
    {
        targetBall = ball;
        collected = false;
    }

    public void StopChasing()
    {
        targetBall = null;
        collected = false;
        if (playerMotion != null) playerMotion.SetMovementSpeed(0f);
    }

    private void Update()
    {
        if (targetBall == null || collected)
        {
            if (playerMotion != null) playerMotion.SetMovementSpeed(0f);
            return;
        }

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
            if (playerMotion != null) playerMotion.SetMovementSpeed(1f);
            transform.position = Vector3.MoveTowards(
                transform.position, target, moveSpeed * Time.deltaTime);
            return;
        }

        collected = true;
        if (playerMotion != null)
        {
            playerMotion.SetMovementSpeed(0f);
            playerMotion.PlayCatch();
        }
        onBallCollected?.Invoke();
    }
}
