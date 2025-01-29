/*
 * Description: simple state machine to use
 * Author: Thorin
 * Note: the states behavior is intended to be ovveridden in each entity
 */
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public abstract class EntityState : MonoBehaviour
{
    protected enum State { Idle, Walking, Running }
    protected int _phase = 1;
    protected State _currentState = State.Idle;
    protected Coroutine _attackCycle;

    [SerializeField] protected string _firstTrack;
    [SerializeField] protected string _secondTrack;

    protected bool _isRitual = false;
    protected int _phaseAnim = Animator.StringToHash("phase");
    protected int _stateAnim = Animator.StringToHash("passive");
    
    [Header("Move parameters")]
    [SerializeField] protected float _speed;
    [SerializeField] private float _maxMoveDistance;
    protected bool _moveWaiting = false;

    [Header("Attack icons")]
    [SerializeField] protected GameObject _attackIcon;
    [SerializeField] protected List<Sprite> _attackActiveIcons;
    [SerializeField] protected List<Sprite> _attackPassiveIcons;

    protected Transform _player;
    protected NavMeshAgent _navMeshAgent;
    protected HealthManager _healthManager;
    protected Animator _animator;

    protected PoolManager _poolManager;
    protected GameManager _gameManager;
    protected AudioManager _audioManager;

    [SerializeField] private string _bossRoar;

    void Start()
    {
        _healthManager = GetComponent<HealthManager>();
        _animator = GetComponent<Animator>();

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _navMeshAgent.speed = _speed;

        _player = GameObject.FindAnyObjectByType<Movement>().transform;

        _poolManager = FindAnyObjectByType<PoolManager>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _audioManager = FindAnyObjectByType<AudioManager>();

        Initialize();
    }
    private void Update()
    {
        switch (_currentState)
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
    protected abstract IEnumerator StartAttacks();
    protected void ChangeState(State newState)
    {
        _currentState = newState;
    }

    protected void StartGame()
    {
        StartCoroutine(BossRoar());
        StartCoroutine(PlayTrackWithDelay(_firstTrack));
        _attackCycle = StartCoroutine(StartAttacks());
    }

    protected IEnumerator PlayTrackWithDelay(string _name, float _duration = 1.8f)
    {
        yield return new WaitForSeconds(_duration);
        _audioManager.PlayBGM(_name);
    }
    #region Boss moving

    protected void FlipToPlayer()
    {
        Vector2 direction = _player.position - transform.position;
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y);
        }
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

    protected IEnumerator MoveWaiting()
    {
        _moveWaiting = true;
        yield return new WaitForSeconds(Random.Range(0.4f, 1.4f));
        if (!_isRitual)
        {
            MoveToRandomPoint();
        }
        _moveWaiting = false;
    }
    void MoveToRandomPoint()
    {
        Vector3 randomPoint;
        for (int i = 0; i < 100; i++)
        {
            randomPoint = Random.insideUnitCircle * _maxMoveDistance;

            if (IsPointReachable(randomPoint))
            {
                _navMeshAgent.SetDestination(randomPoint);
                break;
            }
        }
    }
    bool IsPointReachable(Vector2 point)
    {
        NavMeshHit hit;
        if (NavMesh.Raycast(transform.position, point, out hit, NavMesh.AllAreas))
        {
            if (hit.position != transform.position) return false;
        }
        return true;
    }
    #endregion

    #region BossSigns
    protected IEnumerator BossRoar()
    {
        CameraShake._instance.Shake(1.0f, 1f);
        _audioManager.PlaySFX("Boss Growl");

        GameObject _currentRoar = _poolManager.GetObject(_bossRoar);
        _currentRoar.transform.position = transform.position;
        yield return new WaitForSeconds(0.6f);
        _currentRoar = _poolManager.GetObject(_bossRoar);
        _currentRoar.transform.position = transform.position;
    }
    protected void SignToNextAttack(int attackIndex, bool active)
    {
        GameObject currentAttackIcon = Instantiate(_attackIcon, transform);
        if(active)
            currentAttackIcon.GetComponentInChildren<Image>().sprite = _attackActiveIcons[attackIndex];
        else
            currentAttackIcon.GetComponentInChildren<Image>().sprite = _attackPassiveIcons[attackIndex];
        Destroy(currentAttackIcon, 2f);
    }
    #endregion

    protected abstract void HandleIdle();
    protected abstract void HandleWalking();
    protected abstract void HandleRunning();
    protected abstract void Initialize();
}
