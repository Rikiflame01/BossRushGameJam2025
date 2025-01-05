/*
 * Description: simple state machine to use
 * Author: Thorin
 * Note: the states behavior is intended to be ovveridden in each entity
 */
using UnityEngine;

public abstract class EntityState : MonoBehaviour
{
    protected enum State { Idle, Walking, Running }

    protected State currentState = State.Idle;

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Walking:
                HandleWalking();
                break;
            case State.Running:
                HandleRunning();
                break;
        }
    }

    protected void ChangeState(State newState)
    {
        currentState = newState;
    }

    protected abstract void HandleIdle();
    protected abstract void HandleWalking();
    protected abstract void HandleRunning();
}
