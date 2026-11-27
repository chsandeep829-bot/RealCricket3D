using UnityEngine;

public class BowlingVariationController : MonoBehaviour
{
    public enum DeliveryType { Fast, Swing, OffSpin, LegSpin, Slow }

    [System.Serializable]
    public class DeliveryProfile
    {
        public DeliveryType type;
        [Min(1f)] public float speed = 24f;
        public Vector3 spin = Vector3.zero;
        [Range(0f, 1f)] public float variationAmount;
    }

    [Header("References")]
    [SerializeField] private CricketBall ballPrefab;
    [SerializeField] private Transform releasePoint;
    [SerializeField] private DeliveryProfile[] profiles;

    public void Bowl(DeliveryType type)
    {
        if (ballPrefab == null || releasePoint == null)
        {
            Debug.LogWarning("Assign a ball prefab and release point.");
            return;
        }

        DeliveryProfile profile = FindProfile(type);
        if (profile == null)
        {
            Debug.LogWarning($"No bowling profile configured for {type}.");
            return;
        }

        CricketBall ball = Instantiate(ballPrefab, releasePoint.position, releasePoint.rotation);
        Vector3 direction = releasePoint.forward;

        // Small, readable prototype variation. Tune with the pitch and ball scale.
        if (profile.variationAmount > 0f)
        {
            float side = Random.Range(-profile.variationAmount, profile.variationAmount);
            direction = (direction + releasePoint.right * side).normalized;
        }

        ball.Bowl(direction * profile.speed);
        if (ball.Body != null)
            ball.Body.angularVelocity = releasePoint.TransformDirection(profile.spin);
    }

    public void BowlFast() => Bowl(DeliveryType.Fast);
    public void BowlSwing() => Bowl(DeliveryType.Swing);
    public void BowlOffSpin() => Bowl(DeliveryType.OffSpin);
    public void BowlLegSpin() => Bowl(DeliveryType.LegSpin);
    public void BowlSlow() => Bowl(DeliveryType.Slow);

    private DeliveryProfile FindProfile(DeliveryType type)
    {
        if (profiles == null) return null;
        foreach (DeliveryProfile profile in profiles)
            if (profile != null && profile.type == type) return profile;
        return null;
    }
}
