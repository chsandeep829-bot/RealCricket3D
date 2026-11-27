using System.Collections;
using UnityEngine;

public class CricketCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -8f);
    [SerializeField, Min(0.01f)] private float positionSmoothTime = 0.18f;
    [SerializeField, Min(0.1f)] private float lookHeight = 1.2f;

    private Vector3 velocity;
    private Transform defaultTarget;
    private Coroutine returnRoutine;

    private void Start()
    {
        if (defaultTarget == null) defaultTarget = target;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (defaultTarget == null) defaultTarget = newTarget;
    }

    /// <summary>
    /// Temporarily tracks a moving ball, then restores the normal batter camera.
    /// </summary>
    public void FollowTemporarily(Transform newTarget, float duration)
    {
        if (newTarget == null) return;
        if (defaultTarget == null) defaultTarget = target;
        if (returnRoutine != null) StopCoroutine(returnRoutine);

        target = newTarget;
        returnRoutine = StartCoroutine(ReturnToDefaultAfter(newTarget, Mathf.Max(0.1f, duration)));
    }

    private IEnumerator ReturnToDefaultAfter(Transform followedTarget, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (target == followedTarget) target = defaultTarget;
        returnRoutine = null;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.TransformPoint(offset);
        transform.position = Vector3.SmoothDamp(
            transform.position, desiredPosition, ref velocity, positionSmoothTime);

        Vector3 lookPoint = target.position + Vector3.up * lookHeight;
        Vector3 lookDirection = lookPoint - transform.position;
        if (lookDirection.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
