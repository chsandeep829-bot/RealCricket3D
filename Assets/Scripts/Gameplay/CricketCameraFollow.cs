using UnityEngine;

public class CricketCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -8f);
    [SerializeField, Min(0.01f)] private float positionSmoothTime = 0.18f;
    [SerializeField, Min(0.1f)] private float lookHeight = 1.2f;

    private Vector3 velocity;

    public void SetTarget(Transform newTarget) => target = newTarget;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.TransformPoint(offset);
        transform.position = Vector3.SmoothDamp(
            transform.position, desiredPosition, ref velocity, positionSmoothTime);

        Vector3 lookPoint = target.position + Vector3.up * lookHeight;
        transform.rotation = Quaternion.LookRotation(lookPoint - transform.position);
    }
}
