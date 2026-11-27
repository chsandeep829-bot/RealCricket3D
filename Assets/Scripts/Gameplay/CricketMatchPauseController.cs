using UnityEngine;
using UnityEngine.Events;

public class CricketMatchPauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private UnityEvent onPaused;
    [SerializeField] private UnityEvent onResumed;

    public bool IsPaused { get; private set; }

    private void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        onPaused?.Invoke();
    }

    public void Resume()
    {
        if (!IsPaused) return;
        Time.timeScale = 1f;
        IsPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        onResumed?.Invoke();
    }

    public void TogglePause()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    private void OnDisable()
    {
        if (IsPaused)
        {
            Time.timeScale = 1f;
            IsPaused = false;
        }
    }
}
