using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform bat;
    [SerializeField] private Transform shotDirection;
    [SerializeField] private CricketPlayerAnimator playerAnimator;\n    [SerializeField] private ProceduralCricketPlayerMotion proceduralMotion;

    [Header("Swing")]
    [SerializeField, Min(0.05f)] private float swingDuration = 0.22f;
    [SerializeField] private Vector3 swingRotation = new Vector3(-75f, 0f, 0f);
    [SerializeField, Min(0f)] private float hitImpulse = 18f;
    [SerializeField, Min(0f)] private float lift = 0.25f;
    [SerializeField, Range(0f, 1f)] private float sweetSpotTime = 0.48f;
    [SerializeField, Range(0.01f, 0.5f)] private float timingWindow = 0.24f;
    [SerializeField] private Text timingFeedback;

    private float shotPowerMultiplier = 1f;
    private Quaternion restingRotation;
    private bool isSwinging;
    private float swingProgress;

    private void Awake()
    {
        if (bat != null) restingRotation = bat.localRotation;
    }

    private void Update()
    {
        // Editor prototype input. Connect this action to a mobile UI button.
        if (Input.GetKeyDown(KeyCode.Space)) Swing();
    }

    public void SetShotPowerMultiplier(float multiplier)
    {
        shotPowerMultiplier = Mathf.Clamp(multiplier, 0f, 2f);
    }

    public void Swing()
    {
        if (bat == null || isSwinging) return;
        StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        isSwinging = true;
        swingProgress = 0f;
        if (playerAnimator != null) playerAnimator.PlayBat();\n        if (proceduralMotion != null) proceduralMotion.PlayBat();
        Quaternion target = restingRotation * Quaternion.Euler(swingRotation);
        float elapsed = 0f;
        float totalDuration = swingDuration * 2f;

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            swingProgress = Mathf.Clamp01(elapsed / totalDuration);
            bat.localRotation = Quaternion.Slerp(restingRotation, target, elapsed / swingDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            swingProgress = 0.5f + 0.5f * Mathf.Clamp01(elapsed / swingDuration);
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

        float distanceFromSweetSpot = Mathf.Abs(swingProgress - sweetSpotTime);
        float timingQuality = 1f - Mathf.Clamp01(distanceFromSweetSpot / timingWindow);
        float contactPower = Mathf.Lerp(0.45f, 1.2f, timingQuality);
        Vector3 direction = shotDirection != null ? shotDirection.forward : transform.forward;
        direction = (direction + Vector3.up * lift).normalized;
        ball.Hit(direction * hitImpulse * shotPowerMultiplier * contactPower);

        if (timingFeedback != null)
        {
            if (timingQuality >= 0.82f) timingFeedback.text = "PERFECT";
            else if (timingQuality >= 0.5f) timingFeedback.text = "GOOD";
            else if (timingQuality > 0.05f) timingFeedback.text = "LATE / EARLY";
            else timingFeedback.text = "MISTIMED";
        }
    }
}
