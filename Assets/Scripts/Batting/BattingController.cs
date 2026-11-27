using System.Collections;
using UnityEngine;

public class BattingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform bat;
    [SerializeField] private Transform shotDirection;

    [Header("Swing")]
    [SerializeField, Min(0.05f)] private float swingDuration = 0.22f;
    [SerializeField] private Vector3 swingRotation = new Vector3(-75f, 0f, 0f);
    [SerializeField, Min(0f)] private float hitImpulse = 18f;
    [SerializeField, Min(0f)] private float lift = 0.25f;

    private Quaternion restingRotation;
    private bool isSwinging;

    private void Awake()
    {
        if (bat != null) restingRotation = bat.localRotation;
    }

    private void Update()
    {
        // Editor prototype input. Connect this action to a mobile UI button.
        if (Input.GetKeyDown(KeyCode.Space))
            Swing();
    }

    public void Swing()
    {
        if (bat == null || isSwinging) return;
        StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        isSwinging = true;
        Quaternion target = restingRotation * Quaternion.Euler(swingRotation);
        float elapsed = 0f;

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            bat.localRotation = Quaternion.Slerp(restingRotation, target, elapsed / swingDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            bat.localRotation = Quaternion.Slerp(target, restingRotation, elapsed / swingDuration);
            yield return null;
        }

        bat.localRotation = restingRotation;
        isSwinging = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isSwinging) return;

        CricketBall ball = collision.collider.GetComponentInParent<CricketBall>();
        if (ball == null || ball.HasBeenHit) return;

        Vector3 direction = shotDirection != null ? shotDirection.forward : transform.forward;
        direction = (direction + Vector3.up * lift).normalized;
        ball.Hit(direction * hitImpulse);
    }
}
