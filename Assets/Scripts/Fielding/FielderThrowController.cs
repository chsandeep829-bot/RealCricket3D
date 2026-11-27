using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class FielderThrowController : MonoBehaviour
{
    [Header("Throw")]
    [SerializeField] private Transform throwTarget;
    [SerializeField, Min(1f)] private float throwSpeed = 22f;
    [SerializeField, Min(0f)] private float upwardArc = 0.12f;
    [SerializeField] private bool allowOnlyWhileBattersRun = true;
    [SerializeField] private RunningBetweenWicketsController runningController;
    [SerializeField] private UnityEvent onBallThrown;

    private float lastThrowTime = -10f;
    [SerializeField, Min(0.1f)] private float throwCooldown = 0.8f;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        CricketBall ball = other.GetComponentInParent<CricketBall>();
        if (ball == null || !ball.HasBeenHit || ball.DismissalRecorded ||
            ball.Body == null || throwTarget == null) return;

        if (allowOnlyWhileBattersRun &&
            (runningController == null || !runningController.IsRunning)) return;

        if (Time.time - lastThrowTime < throwCooldown) return;
        lastThrowTime = Time.time;

        Vector3 direction = (throwTarget.position - ball.transform.position).normalized;
        direction = (direction + Vector3.up * upwardArc).normalized;
        ball.Hit(direction * throwSpeed);
        onBallThrown?.Invoke();
    }
}
