using UnityEngine;

/// <summary>
/// Lightweight broadcast camera director. Assign original camera rigs in the Inspector.
/// Camera cuts are event-driven so match and boundary systems can request presentation changes.
/// </summary>
public class CricketBroadcastDirector : MonoBehaviour
{
    [SerializeField] private Camera[] cameras;
    [SerializeField] private CricketCameraFollow ballFollowCamera;
    [SerializeField] private Transform ballTarget;

    private int activeCamera = -1;

    private void Start()
    {
        if (cameras != null && cameras.Length > 0) ActivateCamera(0);
    }

    public void ActivateCamera(int index)
    {
        if (cameras == null || index < 0 || index >= cameras.Length || cameras[index] == null)
            return;

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != null) cameras[i].gameObject.SetActive(i == index);
        }
        activeCamera = index;
    }

    public void ShowBroadcastView() => ActivateCamera(0);
    public void ShowBowlerView() => ActivateCamera(1);
    public void ShowBattingView() => ActivateCamera(2);

    public void FollowBall()
    {
        if (ballFollowCamera != null && ballTarget != null)
        {
            ballFollowCamera.SetTarget(ballTarget);
            ballFollowCamera.gameObject.SetActive(true);
        }
    }

    public void StopBallFollow()
    {
        if (ballFollowCamera != null) ballFollowCamera.gameObject.SetActive(false);
        if (activeCamera >= 0) ActivateCamera(activeCamera);
    }

    public void SetBallTarget(Transform target) => ballTarget = target;
}
