/*
 * Description: simple state machine to use
 * Author: Thorin
 * Note: the states behavior is intended to be ovveridden in each entity
 */
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public abstract class EntityState : MonoBehaviour
{
    protected enum State { Idle, Walking, Running }
    protected int _phase = 1;
    protected State currentState = State.Idle;

    [Header("Move parameters")]
    [SerializeField] protected float _speed;

    protected Transform _player;
    protected NavMeshAgent _navMeshAgent;
    protected HealthManager _healthManager;
    protected Animator _animator;

    void Start()
    {
        _healthManager = GetComponent<HealthManager>();
        _animator = GetComponent<Animator>();

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _navMeshAgent.speed = _speed;

        _player = GameObject.FindAnyObjectByType<Movement>().transform;

        Initialize();
    }
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

    protected void DisableMovement()
    {
        _navMeshAgent.speed = 0f;
    }
    protected void EnableMovement()
    {
        _navMeshAgent.speed = _speed;
    }

    protected IEnumerator DisableMovementForTime(float duration)
    {
        DisableMovement();
        yield return new WaitForSeconds(duration);
        EnableMovement();
    }
    protected abstract void HandleIdle();
    protected abstract void HandleWalking();
    protected abstract void HandleRunning();
    protected abstract void Initialize();
}
