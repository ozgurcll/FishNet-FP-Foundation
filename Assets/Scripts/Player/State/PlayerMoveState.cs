using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.motor.Move(player.moveInput);
    }

    public override void Exit()
    {
        base.Exit();
        player.motor.Stop();
    }
}