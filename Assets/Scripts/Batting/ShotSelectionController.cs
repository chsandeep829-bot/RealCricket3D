using UnityEngine;

public class ShotSelectionController : MonoBehaviour
{
    public enum ShotType { Defensive, Drive, CoverDrive, Pull, Cut, Sweep, Lofted, Stroke, Push, Leave }

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
    public void SelectStroke() => Select(ShotType.Stroke);
    public void SelectPush() => Select(ShotType.Push);
    public void SelectLeave() => Select(ShotType.Leave);

    public void SetAim(float normalizedAim)
    {
        aimAngle = Mathf.Lerp(-80f, 80f, Mathf.Clamp01((normalizedAim + 1f) * 0.5f));
        ApplyAim();
    }

    /// <summary>
    /// A swipe aims the currently selected shot without changing its type.
    /// Loft, Stroke, Push and Leave remain separate selections.
    /// </summary>
    public void PlaySwipeShot(Vector2 swipe)
    {
        if (SelectedShot == ShotType.Leave) return;
        aimAngle = Mathf.Clamp(swipe.x, -1f, 1f) * 70f;
        ApplyAim();
        PlaySelectedShot();
    }

    public void PlaySelectedShot()
    {
        if (SelectedShot == ShotType.Leave) return;
        ApplyAim();
        if (battingController != null) battingController.Swing();
    }

    private void Select(ShotType shot)
    {
        SelectedShot = shot;
        if (battingController != null) battingController.SetShotPowerMultiplier(1f);
        switch (shot)
        {
            case ShotType.Defensive: loftAngle = 2f; break;
            case ShotType.Drive: loftAngle = 10f; break;
            case ShotType.CoverDrive: loftAngle = 14f; aimAngle = -25f; break;
            case ShotType.Pull: loftAngle = 18f; aimAngle = 35f; break;
            case ShotType.Cut: loftAngle = 8f; aimAngle = 55f; break;
            case ShotType.Sweep: loftAngle = 5f; aimAngle = 70f; break;
            case ShotType.Lofted: loftAngle = 38f; if (battingController != null) battingController.SetShotPowerMultiplier(1.15f); break;
            case ShotType.Stroke: loftAngle = 10f; aimAngle = 0f; break;
            case ShotType.Push: loftAngle = 3f; aimAngle = 0f; if (battingController != null) battingController.SetShotPowerMultiplier(0.55f); break;
            case ShotType.Leave: loftAngle = 0f; break;
        }
        ApplyAim();
    }

    private void ApplyAim()
    {
        if (shotDirection == null) return;
        shotDirection.localRotation = Quaternion.Euler(-loftAngle, aimAngle, 0f);
    }
}
