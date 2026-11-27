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

    [Header("Bowling controls")]
    [SerializeField, Range(-1f, 1f)] private float line;
    [SerializeField, Range(0f, 1f)] private float length = 0.5f;
    [SerializeField, Range(-12f, 12f)] private float maxLineAngle = 8f;
    [SerializeField] private Vector2 lengthAngleRange = new Vector2(-3f, 9f);

    public void SetLine(float normalizedLine) => line = Mathf.Clamp(normalizedLine, -1f, 1f);
    public void SetLength(float normalizedLength) => length = Mathf.Clamp01(normalizedLength);

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
        float variation = profile.variationAmount > 0f
            ? Random.Range(-profile.variationAmount, profile.variationAmount) : 0f;
        float lineAngle = line * maxLineAngle + variation * maxLineAngle;
        float lengthAngle = Mathf.Lerp(lengthAngleRange.x, lengthAngleRange.y, length);
        Vector3 direction = Quaternion.AngleAxis(lineAngle, releasePoint.up)
            * Quaternion.AngleAxis(lengthAngle, releasePoint.right)
            * releasePoint.forward;

        ball.Bowl(direction.normalized * profile.speed);
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
