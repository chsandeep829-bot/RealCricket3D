using UnityEngine;

public class MobileCricketControls : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private BowlingController bowlingController;
    [SerializeField] private BattingController battingController;

    // Connect these public methods to Unity UI Button OnClick events.
    public void OnBowlPressed()
    {
        if (bowlingController != null) bowlingController.BowlDelivery();
    }

    public void OnBatPressed()
    {
        if (battingController != null) battingController.Swing();
    }
}
