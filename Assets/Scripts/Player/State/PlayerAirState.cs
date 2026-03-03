using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        player.motor.Move(player.moveInput);

        if (player.motor.IsGrounded() && player.rb.linearVelocity.y <= 0.1f)
        {
            if (player.moveInput != Vector2.zero)
                stateMachine.ChangeState(player.moveState);
            else
                stateMachine.ChangeState(player.idleState);
        }
    }
}
