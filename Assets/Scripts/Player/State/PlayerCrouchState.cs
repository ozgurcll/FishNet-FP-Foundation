using UnityEngine;

public class PlayerCrouchState : PlayerState
{
    public PlayerCrouchState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.motor.Move(player.moveInput);
    }
}
