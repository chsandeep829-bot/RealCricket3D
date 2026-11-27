using UnityEngine;

/// <summary>
/// Lightweight procedural animation for the prototype cricket avatars.
/// Replace generated mesh parts with a licensed humanoid rig and clips later.
/// </summary>
public sealed class ProceduralCricketPlayerMotion : MonoBehaviour
{
    [Header("Rig")]
    [SerializeField] private Transform torso;
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;
    [SerializeField] private Transform leftLeg;
    [SerializeField] private Transform rightLeg;

    [Header("Motion")]
    [SerializeField, Min(0f)] private float runFrequency = 10f;
    [SerializeField, Min(0f)] private float runArmAngle = 48f;
    [SerializeField, Min(0f)] private float runLegAngle = 34f;

    private Quaternion torsoRest, headRest, leftArmRest, rightArmRest, leftLegRest, rightLegRest;
    private float movementSpeed;
    private float actionTimer;
    private float actionDuration;
    private ActionPose actionPose;

    private enum ActionPose { None, Bat, Bowl, Throw, Catch, Dive }

    private void Awake() => CacheRestPose();

    public void Configure(Transform body, Transform headBone, Transform leftArmBone,
        Transform rightArmBone, Transform leftLegBone, Transform rightLegBone)
    {
        torso = body;
        head = headBone;
        leftArm = leftArmBone;
        rightArm = rightArmBone;
        leftLeg = leftLegBone;
        rightLeg = rightLegBone;
        CacheRestPose();
    }

    public void SetMovementSpeed(float normalizedSpeed)
    {
        movementSpeed = Mathf.Clamp01(normalizedSpeed);
    }

    public void ResetMotion()
    {
        movementSpeed = 0f;
        actionPose = ActionPose.None;
        actionTimer = 0f;
        ApplyRestPose();
    }

    public void PlayBat() => BeginAction(ActionPose.Bat, 0.48f);
    public void PlayBowl() => BeginAction(ActionPose.Bowl, 0.72f);
    public void PlayThrow() => BeginAction(ActionPose.Throw, 0.46f);
    public void PlayCatch() => BeginAction(ActionPose.Catch, 0.38f);
    public void PlayDive() => BeginAction(ActionPose.Dive, 0.62f);

    private void BeginAction(ActionPose pose, float duration)
    {
        actionPose = pose;
        actionDuration = duration;
        actionTimer = 0f;
    }

    private void Update()
    {
        float run = Mathf.Sin(Time.time * runFrequency) * movementSpeed;
        if (torso != null) torso.localRotation = torsoRest * Quaternion.Euler(Mathf.Abs(run) * 2f, 0f, 0f);
        if (head != null) head.localRotation = headRest * Quaternion.Euler(-Mathf.Abs(run) * 1.5f, 0f, 0f);
        if (leftLeg != null) leftLeg.localRotation = leftLegRest * Quaternion.Euler(run * runLegAngle, 0f, 0f);
        if (rightLeg != null) rightLeg.localRotation = rightLegRest * Quaternion.Euler(-run * runLegAngle, 0f, 0f);
        if (leftArm != null) leftArm.localRotation = leftArmRest * Quaternion.Euler(-run * runArmAngle, 0f, 0f);
        if (rightArm != null) rightArm.localRotation = rightArmRest * Quaternion.Euler(run * runArmAngle, 0f, 0f);

        if (actionPose == ActionPose.None) return;
        actionTimer += Time.deltaTime;
        float t = Mathf.Clamp01(actionTimer / actionDuration);
        ApplyActionPose(actionPose, Mathf.Sin(t * Mathf.PI));
        if (t >= 1f) actionPose = ActionPose.None;
    }

    private void ApplyActionPose(ActionPose pose, float weight)
    {
        float left = 0f, right = 0f, torsoAngle = 0f;
        switch (pose)
        {
            case ActionPose.Bat: left = -78f; right = -112f; torsoAngle = -24f; break;
            case ActionPose.Bowl: left = -115f; right = 65f; torsoAngle = 32f; break;
            case ActionPose.Throw: left = -18f; right = -105f; torsoAngle = 18f; break;
            case ActionPose.Catch: left = -105f; right = -105f; torsoAngle = -8f; break;
            case ActionPose.Dive: left = -72f; right = 72f; torsoAngle = 62f; break;
        }
        if (leftArm != null) leftArm.localRotation = leftArmRest * Quaternion.Euler(left * weight, 0f, -12f * weight);
        if (rightArm != null) rightArm.localRotation = rightArmRest * Quaternion.Euler(right * weight, 0f, 12f * weight);
        if (torso != null) torso.localRotation = torsoRest * Quaternion.Euler(torsoAngle * weight, 0f, 0f);
    }

    private void CacheRestPose()
    {
        if (torso != null) torsoRest = torso.localRotation;
        if (head != null) headRest = head.localRotation;
        if (leftArm != null) leftArmRest = leftArm.localRotation;
        if (rightArm != null) rightArmRest = rightArm.localRotation;
        if (leftLeg != null) leftLegRest = leftLeg.localRotation;
        if (rightLeg != null) rightLegRest = rightLeg.localRotation;
    }

    private void ApplyRestPose()
    {
        if (torso != null) torso.localRotation = torsoRest;
        if (head != null) head.localRotation = headRest;
        if (leftArm != null) leftArm.localRotation = leftArmRest;
        if (rightArm != null) rightArm.localRotation = rightArmRest;
        if (leftLeg != null) leftLeg.localRotation = leftLegRest;
        if (rightLeg != null) rightLeg.localRotation = rightLegRest;
    }
}
