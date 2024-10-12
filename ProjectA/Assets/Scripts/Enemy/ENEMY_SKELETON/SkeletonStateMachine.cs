using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonStateMachine
{
    private Skeleton owner;

    public IState PreviousState { get; private set; }
    public IState CurrentState { get; private set; }

    //state ¸ñ·Ï
    public SkeletonIdleState idleState;
    public SkeletonMoveState moveState;
    //public IState<T> attackOneState;
    //public IState<T> attackTwoState;
    //public IState<T> attackThreeState;
    //public IState<T> hurtState;

    public SkeletonStateMachine(Skeleton _skeleton)
    {
        owner = _skeleton;
    }

    public void Initialize(IState _startingState)
    {
        CurrentState = _startingState;
        _startingState.Enter();
    }

    public void TransitionTo(IState _nextState)
    {
        CurrentState.Exit();
        CurrentState = _nextState;
        _nextState.Enter();
    }

    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
}
