using UnityEngine;
using System.Collections;

public class FiniteStateMachine<T> {
    public T entity;
    public State<T> currentState;

    public FiniteStateMachine(T entity, State<T> startingState)
    {
        this.entity = entity;
        this.currentState = startingState;
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Execute(entity);
        }
    }

    public void ChangeState(State<T> newState)
    {
        currentState = newState;
        Update();
    }
}
