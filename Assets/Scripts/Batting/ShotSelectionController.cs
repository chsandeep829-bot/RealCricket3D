using UnityEngine;

public class ShotSelectionController : MonoBehaviour
{
    public enum ShotType { Defensive, Drive, CoverDrive, Pull, Cut, Sweep, Lofted }

    [Header("References")]
    [SerializeField] private BattingController battingController;
    [SerializeField] private Transform shotDirection;

    [Header("Aim")]
    [SerializeField, Range(-80f, 80f)] private float aimAngle = 0f;
    [SerializeField, Min(0f)] private float loftAngle = 12f;

    public ShotType SelectedShot { get; private set; } = ShotType.Drive;

    public void SelectDefensive() => Select(ShotType.Defensive);
    public void SelectDrive() => Select(ShotType.Drive);
    public void SelectCoverDrive() => Select(ShotType.CoverDrive);
    public void SelectPull() => Select(ShotType.Pull);
    public void SelectCut() => Select(ShotType.Cut);
    public void SelectSweep() => Select(ShotType.Sweep);
    public void SelectLofted() => Select(ShotType.Lofted);

    public void SetAim(float normalizedAim)
    {
        aimAngle = Mathf.Lerp(-80f, 80f, Mathf.Clamp01((normalizedAim + 1f) * 0.5f));
        ApplyAim();
    }

    /// <summary>
    /// Maps a screen swipe to a shot type, aim direction and loft, then plays it.
    /// Swipe up adds loft; swipe down favours a grounded defensive shot.
    /// Horizontal movement aims the shot across the field.
    /// </summary>
    public void PlaySwipeShot(Vector2 swipe)
    {
        float horizontal = Mathf.Clamp(swipe.x, -1f, 1f);
        float vertical = Mathf.Clamp(swipe.y, -1f, 1f);

        if (vertical > 0.55f)
            Select(ShotType.Lofted);
        else if (vertical < -0.55f)
            Select(ShotType.Defensive);
        else if (horizontal > 0.45f)
            Select(ShotType.Pull);
        else if (horizontal < -0.45f)
            Select(ShotType.Cut);
        else
            Select(ShotType.Drive);

        aimAngle = horizontal * 70f;
        loftAngle = Mathf.Clamp(Mathf.Lerp(3f, 42f, (vertical + 1f) * 0.5f), 2f, 42f);
        ApplyAim();

        if (battingController != null)
            battingController.Swing();
    }

    public void PlaySelectedShot()
    {
        ApplyAim();
        if (battingController != null) battingController.Swing();
    }

    private void Select(ShotType shot)
    {
        SelectedShot = shot;
        switch (shot)
        {
            case ShotType.Defensive: loftAngle = 2f; break;
            case ShotType.Drive: loftAngle = 10f; break;
            case ShotType.CoverDrive: loftAngle = 14f; aimAngle = -25f; break;
            case ShotType.Pull: loftAngle = 18f; aimAngle = 35f; break;
            case ShotType.Cut: loftAngle = 8f; aimAngle = 55f; break;
            case ShotType.Sweep: loftAngle = 5f; aimAngle = 70f; break;
            case ShotType.Lofted: loftAngle = 38f; break;
        }
        ApplyAim();
    }

    private void ApplyAim()
    {
        if (shotDirection == null) return;
        shotDirection.localRotation = Quaternion.Euler(-loftAngle, aimAngle, 0f);
    }
}
