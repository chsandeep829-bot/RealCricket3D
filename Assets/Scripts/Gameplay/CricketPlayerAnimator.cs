using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CricketPlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private string throwTrigger = "Throw";
    [SerializeField] private string catchTrigger = "Catch";
    [SerializeField] private string batTrigger = "Bat";
    [SerializeField] private string bowlTrigger = "Bowl";
    [SerializeField] private string diveTrigger = "Dive";

    private int speedHash;
    private int throwHash;
    private int catchHash;
    private int batHash;
    private int bowlHash;
    private int diveHash;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        speedHash = Animator.StringToHash(speedParameter);
        throwHash = Animator.StringToHash(throwTrigger);
        catchHash = Animator.StringToHash(catchTrigger);
        batHash = Animator.StringToHash(batTrigger);
        bowlHash = Animator.StringToHash(bowlTrigger);
        diveHash = Animator.StringToHash(diveTrigger);
    }

    public void SetMovementSpeed(float normalizedSpeed)
    {
        if (animator != null) animator.SetFloat(speedHash, Mathf.Clamp01(normalizedSpeed));
    }

    public void PlayThrow() => Trigger(throwHash);
    public void PlayCatch() => Trigger(catchHash);
    public void PlayBat() => Trigger(batHash);
    public void PlayBowl() => Trigger(bowlHash);
    public void PlayDive() => Trigger(diveHash);

    public void ResetMotion()
    {
        if (animator != null) animator.SetFloat(speedHash, 0f);
    }

    private void Trigger(int hash)
    {
        if (animator != null) animator.SetTrigger(hash);
    }
}
