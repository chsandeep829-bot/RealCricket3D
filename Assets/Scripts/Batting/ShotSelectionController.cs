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
