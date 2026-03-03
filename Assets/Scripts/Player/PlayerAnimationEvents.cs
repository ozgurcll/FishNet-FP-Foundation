using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private Player _player;

    void Start()
    {
        _player = GetComponentInParent<Player>();
    }

    // public void RightFX() => _player.fx.PlayRightFootstepEffect();

    // public void LeftFX() => _player.fx.PlayLeftFootstepEffect();
    // public void JumpFX() => _player.fx.JumpEffect();

    private void AnimationTrigger() => _player.AnimationTrigger();

}
