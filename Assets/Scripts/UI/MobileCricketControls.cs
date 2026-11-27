using UnityEngine;

public class MobileCricketControls : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private BowlingController bowlingController;
    [SerializeField] private BowlingVariationController bowlingVariations;
    [SerializeField] private BattingController battingController;
    [SerializeField] private ShotSelectionController shotSelection;
    [SerializeField] private RunningBetweenWicketsController runningController;

    // Connect these public methods to Unity UI Button/Slider events.
    public void OnBowlPressed()
    {
        if (bowlingController != null) bowlingController.BowlDelivery();
    }

    public void OnFastBowlPressed()
    {
        if (bowlingVariations != null) bowlingVariations.BowlFast();
        else OnBowlPressed();
    }

    public void OnSwingBowlPressed()
    {
        if (bowlingVariations != null) bowlingVariations.BowlSwing();
        else OnBowlPressed();
    }

    public void OnOffSpinBowlPressed()
    {
        if (bowlingVariations != null) bowlingVariations.BowlOffSpin();
        else OnBowlPressed();
    }

    public void OnLegSpinBowlPressed()
    {
        if (bowlingVariations != null) bowlingVariations.BowlLegSpin();
        else OnBowlPressed();
    }

    public void OnSlowBowlPressed()
    {
        if (bowlingVariations != null) bowlingVariations.BowlSlow();
        else OnBowlPressed();
    }

    public void OnBowlingLineChanged(float normalizedLine)
    {
        if (bowlingVariations != null) bowlingVariations.SetLine(normalizedLine);
    }

    public void OnBowlingLengthChanged(float normalizedLength)
    {
        if (bowlingVariations != null) bowlingVariations.SetLength(normalizedLength);
    }

    public void OnBatPressed()
    {
        if (shotSelection != null) shotSelection.PlaySelectedShot();
        else if (battingController != null) battingController.Swing();
    }

    public void OnDefensiveShot() { if (shotSelection != null) shotSelection.SelectDefensive(); }
    public void OnDriveShot() { if (shotSelection != null) shotSelection.SelectDrive(); }
    public void OnCoverDriveShot() { if (shotSelection != null) shotSelection.SelectCoverDrive(); }
    public void OnPullShot() { if (shotSelection != null) shotSelection.SelectPull(); }
    public void OnCutShot() { if (shotSelection != null) shotSelection.SelectCut(); }
    public void OnSweepShot() { if (shotSelection != null) shotSelection.SelectSweep(); }
    public void OnLoftedShot() { if (shotSelection != null) shotSelection.SelectLofted(); }
    public void OnStrokeShot() { if (shotSelection != null) shotSelection.SelectStroke(); }
    public void OnPushShot() { if (shotSelection != null) shotSelection.SelectPush(); }
    public void OnLeaveBall() { if (shotSelection != null) shotSelection.SelectLeave(); }

    public void OnRunPressed()
    {
        if (runningController != null) runningController.RequestRun();
    }

    public void OnTwoRunsPressed()
    {
        if (runningController != null) runningController.RequestTwoRuns();
    }

    public void OnCancelRunsPressed()
    {
        if (runningController != null) runningController.CancelQueuedRuns();
    }
}
