using UnityEngine;

/// <summary>
/// Forwards collisions from the bat's child collider to the batter controller.
/// This avoids relying on collision messages reaching a parent without a Rigidbody.
/// </summary>
[RequireComponent(typeof(Collider))]
public sealed class BatCollisionRelay : MonoBehaviour
{
    [SerializeField] private BattingController battingController;

    private void Awake()
    {
        if (battingController == null)
            battingController = GetComponentInParent<BattingController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (battingController == null)
            battingController = GetComponentInParent<BattingController>();
        if (battingController != null)
            battingController.TryHitBall(collision);
    }
}
