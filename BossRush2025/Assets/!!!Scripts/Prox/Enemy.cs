using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemySO _enemySO;
    [SerializeField] private MeleeDamage _meleeDamage;
    [SerializeField] private float _appearDelay;

    [SerializeField] private Transform _target;
    [SerializeField] private List<Component> _componentsToDisable;

    private bool _canMove = false, _isAppear = false, _isFlip = false;

    private PoolManager _poolManager;
    private GameManager _gameManager;
    private NavMeshAgent _navMeshAgent;
    private Knockback _knockBack;
    private HealthManager _healthManager;
    private SpriteRenderer _spriteRenderer;
    private Coroutine _disableCoroutine;
    private Animator _animator;

    private int _appearAnim = Animator.StringToHash("appear");

    private ParticleSystem _walkParticles;
    private Vector3 _currentParticlesRotate;

    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _healthManager = GetComponent<HealthManager>();
        _knockBack = GetComponent<Knockback>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _poolManager = FindAnyObjectByType<PoolManager>();
        _gameManager = FindAnyObjectByType<GameManager>();

        _knockBack._onStartKnockback += DisableMovement;
        _knockBack._onFinishKnockback += EnableMovement;

        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _navMeshAgent.speed = _enemySO._speed;

        _healthManager.SetHealth(_enemySO._health);
        _healthManager._onDie += ReturnInPool;
        _meleeDamage.SetDamage(_enemySO._damage);
        _meleeDamage.SetAttackInterval(_enemySO._attackInterval);
        
        _target = GameObject.FindAnyObjectByType<Movement>().transform;
        _canMove = true;

        _walkParticles = GetComponentInChildren<ParticleSystem>();
        _currentParticlesRotate = _walkParticles.transform.eulerAngles;

        _gameManager.PlayerDie += StopChasing;
        _gameManager.RitualStart += DestroyEnemy;
        _gameManager.BossDefeat += DestroyEnemy;
    }
    void Start()
    {
        if(!_isAppear) _disableCoroutine = StartCoroutine(DisableMovementForTime(_appearDelay));
    }
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (!_canMove)
            return;

        bool stopEnemy = false;
        if (_target != null)
        {
            _currentParticlesRotate.x = -Mathf.Atan2(_navMeshAgent.velocity.y, _navMeshAgent.velocity.x) * Mathf.Rad2Deg + 180f;
            _walkParticles.transform.eulerAngles = _currentParticlesRotate;
            if ((_navMeshAgent.velocity.x < 0 && _isFlip) || (_navMeshAgent.velocity.x > 0 && !_isFlip))
            {
                _isFlip = !_isFlip;
                _spriteRenderer.flipX = _isFlip;
            }
            if (Vector2.Distance(transform.position, _target.position) > _enemySO._minStopRadius)
                _navMeshAgent.SetDestination(_target.position);
            else
            {
                stopEnemy = true;
            }    
        }
        else 
        {
            stopEnemy = true;
        }

        if (stopEnemy)
        {
            DisableMovement();
        }
    }
    void OnEnable()
    {
        _healthManager.SetHealth(_enemySO._health);
        _disableCoroutine = StartCoroutine(DisableMovementForTime(_appearDelay));
    }
    void OnDisable()
    {
        EnableMovement();
    }
    public void DisableMovement()
    {
        _canMove = false;
        _navMeshAgent.speed = 0;
        _walkParticles.Stop();
    }

    public void EnableMovement()
    {
        _canMove = true;
        if(_navMeshAgent != null)
            _navMeshAgent.speed = _enemySO._speed;
        _walkParticles.Play();
    }
    private void ReturnInPool()
    {
        _poolManager.ReturnObject(gameObject, _enemySO._name);
    }
    private IEnumerator DisableMovementForTime(float _disableTime)
    {
        _isAppear = true;
        _animator.SetTrigger(_appearAnim);
        DisableMovement();
        foreach (Behaviour component in _componentsToDisable)
        {
            component.enabled = false;
        }
        yield return new WaitForSeconds(_disableTime);
        EnableMovement();
        foreach (Behaviour component in _componentsToDisable)
        {
            component.enabled = true;
        }
        _isAppear = false;
    }
    private void StopChasing()
    {
        if (_disableCoroutine != null)
        {
            StopCoroutine(_disableCoroutine);
            _disableCoroutine = null;
        }
        DisableMovement();
    }
    private void DestroyEnemy()
    {
        _healthManager.TakeDamage(_enemySO._health);
    }
}

