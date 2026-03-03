using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.motor.Jump();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        player.motor.Move(player.moveInput);

        if (player.rb.linearVelocity.y <= 0)
            stateMachine.ChangeState(player.airState);
    }

}