using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RunningBetweenWicketsController : MonoBehaviour
{
    [Header("Batter References")]
    [SerializeField] private Transform strikerBatter;
    [SerializeField] private Transform nonStrikerBatter;
    [SerializeField] private Transform strikerCrease;
    [SerializeField] private Transform nonStrikerCrease;
    [SerializeField] private MatchManager matchManager;

    [Header("Running")]
    [SerializeField, Min(0.1f)] private float runSpeed = 5f;
    [SerializeField, Min(1)] private int maxQueuedRuns = 3;
    [SerializeField] private UnityEvent onRunCompleted;

    private int queuedRuns;
    private bool isRunning;
    private bool stopAfterCurrentRun;

    public bool IsRunning => isRunning;
    public int QueuedRuns => queuedRuns;
    public bool IsStrikerOutOfCrease => strikerBatter != null && strikerCrease != null &&
        Vector3.Distance(strikerBatter.position, strikerCrease.position) > 0.65f;

    // Connect this method to a mobile UI Run button.
    public void RequestRun()
    {
        RequestRuns(1);
    }

    public void RequestTwoRuns()
    {
        RequestRuns(2);
    }

    public void RequestRuns(int count)
    {
        if (matchManager == null || matchManager.IsInningsCompleted ||
            matchManager.IsMatchCompleted || strikerBatter == null ||
            nonStrikerBatter == null || strikerCrease == null ||
            nonStrikerCrease == null || count <= 0) return;

        queuedRuns = Mathf.Min(maxQueuedRuns, queuedRuns + count);
        stopAfterCurrentRun = false;

        if (!isRunning)
            StartCoroutine(RunRoutine());
    }

    // Clears pending runs but lets the current crossing finish safely.
    public void CancelQueuedRuns()
    {
        queuedRuns = 0;
        if (isRunning) stopAfterCurrentRun = true;
    }

    private IEnumerator RunRoutine()
    {
        isRunning = true;

        while (queuedRuns > 0 && !stopAfterCurrentRun &&
               !matchManager.IsInningsCompleted && !matchManager.IsMatchCompleted)
        {
            queuedRuns--;

            Vector3 strikerStart = strikerBatter.position;
            Vector3 nonStrikerStart = nonStrikerBatter.position;
            Vector3 strikerTarget = nonStrikerCrease.position;
            Vector3 nonStrikerTarget = strikerCrease.position;

            float elapsed = 0f;
            float distance = Vector3.Distance(strikerStart, strikerTarget);
            float duration = Mathf.Max(0.1f, distance / runSpeed);
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                strikerBatter.position = Vector3.Lerp(strikerStart, strikerTarget, t);
                nonStrikerBatter.position = Vector3.Lerp(nonStrikerStart, nonStrikerTarget, t);
                yield return null;
            }

            strikerBatter.position = strikerTarget;
            nonStrikerBatter.position = nonStrikerTarget;

            // After crossing, the two batters have exchanged ends.
            Transform previousStrikerCrease = strikerCrease;
            strikerCrease = nonStrikerCrease;
            nonStrikerCrease = previousStrikerCrease;

            matchManager.RecordRunningRuns(1);
            onRunCompleted?.Invoke();
        }

        isRunning = false;
        stopAfterCurrentRun = false;
    }
}
