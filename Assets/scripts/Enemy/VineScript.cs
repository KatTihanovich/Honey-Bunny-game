using UnityEngine;

public class VineScript : MonoBehaviour
{
    public CapsuleCollider2D middleCollider;
    public BoxCollider2D highCollider;
    private HealthNew playerHealth;
    public Animator vineAnimator;

    private enum VineState { Low, Middle, High }
    private VineState currentState;

    private void Start()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<HealthNew>();
            }

            if (playerHealth == null)
            {
                Debug.LogWarning("VineScript: Player's HealthNew component not found.");
            }
        }
        currentState = VineState.Middle;
        UpdateVineState();
    }

    private void Update()
    {
        UpdateVineState();
    }

    private void UpdateVineState()
    {
        if (playerHealth == null || vineAnimator == null)
            return;

        float healthPercent = (playerHealth.CurrentHealth / playerHealth.MaxHealth) * 100f;
        VineState newState = GetVineStateFromHealth();

        if (newState != currentState)
        {
            Debug.Log($"Transitioning from {currentState} to {newState}");
            PlayTransitionAnimation(currentState, newState);
            currentState = newState;
            ApplyColliderState(currentState);
        }
    }

    private void PlayTransitionAnimation(VineState from, VineState to)
    {
        if (from == VineState.Low && to == VineState.Middle)
            vineAnimator.SetTrigger("LowToMid");
        else if (from == VineState.Middle && to == VineState.High)
            vineAnimator.SetTrigger("MidToHigh");
        else if (from == VineState.High && to == VineState.Middle)
            vineAnimator.SetTrigger("HighToMid");
        else if (from == VineState.Middle && to == VineState.Low)
            vineAnimator.SetTrigger("MidToLow");
        else if (from == VineState.High && to == VineState.Low)
            vineAnimator.SetTrigger("HighToLow");
        else if (from == VineState.Low && to == VineState.High)
            vineAnimator.SetTrigger("LowToHigh");    
    }

    private void ApplyColliderState(VineState state)
    {
        switch (state)
        {
            case VineState.Low:
                middleCollider.enabled = false;
                highCollider.enabled = false;
                break;

            case VineState.Middle:
                middleCollider.enabled = true;
                highCollider.enabled = false;
                break;

            case VineState.High:
                middleCollider.enabled = false;
                highCollider.enabled = true;
                break;
        }
    }
    
    private VineState GetVineStateFromHealth()
    {
        float healthPercent = (playerHealth.CurrentHealth / playerHealth.MaxHealth) * 100f;

        if (healthPercent <= 50f)
            return VineState.High;
        else if (healthPercent <= 70f)
            return VineState.Middle;
        else
            return VineState.Low;
    }
}
