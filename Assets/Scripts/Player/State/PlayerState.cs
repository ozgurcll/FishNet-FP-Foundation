using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected Rigidbody rb;
    protected float stateTimer;
    protected string animBoolName;

    protected bool isAnimFinished;

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.anim.Animator.SetBool(animBoolName, true);
        rb = player.rb;
        isAnimFinished = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void FixedUpdate()
    {
        
    }
    
    public virtual void Exit()
    {
        player.anim.Animator.SetBool(animBoolName, false);
    }
    public virtual void AnimationFinishTrigger() => isAnimFinished = true;

}
