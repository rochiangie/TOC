using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public CharacterController controller;

    private void OnEnable()
    {
        GameEvents.OnBagFilled += OnBagFilled;
        GameEvents.OnBagDisposed += OnBagDisposed;
    }

    private void OnDisable()
    {
        GameEvents.OnBagFilled -= OnBagFilled;
        GameEvents.OnBagDisposed -= OnBagDisposed;
    }

    private void Update()
    {
        if (animator == null || controller == null) return;

        bool isWalking = controller.velocity.magnitude > 0.1f;
        animator.SetBool("IsWalking", isWalking);
    }

    private void OnBagFilled()
    {
        if (animator != null) animator.SetBool("IsCarrying", true);
    }

    private void OnBagDisposed()
    {
        if (animator != null) animator.SetBool("IsCarrying", false);
    }
}
