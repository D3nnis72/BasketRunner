using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SwordVFXManager _swordVFXManager;

    private static class AnimationParameters
    {
        public const string Speed = "speed";
        public const string IsWalking = "isWalking";
        public const string Attack = "attack";
    }

    private void EnsureAnimatorAssigned()
    {
        if (_animator == null)
        {
            Debug.LogError("Animator is not assigned.");
        }
    }

    public void PlayWalkingAnimation(float speed)
    {
        EnsureAnimatorAssigned();
        _animator?.SetFloat(AnimationParameters.Speed, speed);
        _animator?.SetBool(AnimationParameters.IsWalking, true);
    }

    public void StopWalkingAnimation()
    {
        EnsureAnimatorAssigned();
        _animator?.SetFloat(AnimationParameters.Speed, 0f);
        _animator?.SetBool(AnimationParameters.IsWalking, false);
    }

    public void PlayAttackAnimation()
    {
        EnsureAnimatorAssigned();
        _animator?.SetTrigger(AnimationParameters.Attack);
    }

    public void DisableAnimator()
    {
        EnsureAnimatorAssigned();
        _animator.enabled = false;
    }

    public void EnableAnimator()
    {
        EnsureAnimatorAssigned();
        _animator.enabled = true;
    }

    private void OnSlashStart()
    {
        // _swordVFXManager?.PlayVFX();
        Debug.Log("Slash animation started.");
    }

    private void OnSlashComplete()
    {
        EnsureAnimatorAssigned();
        _animator?.ResetTrigger(AnimationParameters.Attack);
        // _swordVFXManager?.StopVFX();
        Debug.Log("Slash animation completed.");
    }
}
