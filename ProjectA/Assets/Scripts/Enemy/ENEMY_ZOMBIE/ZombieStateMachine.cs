
using System.Xml;

public class ZombieStateMachine
{
    private Zombie owner;

    public IState PreviousState { get; private set; }
    public IState CurrentState { get; private set; }

    //state ¸ñ·Ï
    public ZombieIdleState idleState;
    public ZombieMoveState moveState;
    public ZombieAttackState attackState;
    public ZombieHurtState hurtState;
    public ZombieDeadState deadState;
    //public IState<T> attackTwoState;
    //public IState<T> attackThreeState;
    //public IState<T> hurtState;

    public ZombieStateMachine(Zombie _enemy)
    {
        owner = _enemy;

        idleState = new ZombieIdleState(owner);
        moveState = new ZombieMoveState(owner);
        attackState = new ZombieAttackState(owner);
        hurtState = new ZombieHurtState(owner);
        deadState = new ZombieDeadState(owner);

        Initialize(idleState);
    }

    public void Initialize(IState _startingState)
    {
        CurrentState = _startingState;
        _startingState.Enter();
    }

    public void TransitionTo(IState _nextState)
    {
        PreviousState = CurrentState;
        CurrentState.Exit();
        CurrentState = _nextState;
        _nextState.Enter();
        owner.IsWaiting = true;
    }

    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
}
