using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("OYUNCU ÖLDÜ");
        player.controls.Disable();
        player.GetComponent<CapsuleCollider>().enabled = false; // Öldüğünde çarpışmayı kapat

        if (player.IsOwner)
        {
            player.equipment.DropEverythingOnDeath();
        }
    }
}