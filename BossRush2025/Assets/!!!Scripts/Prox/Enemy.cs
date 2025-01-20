using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemySO _enemySO;
    [SerializeField] private MeleeDamage _meleeDamage;
    [SerializeField] private float _appearDelay;
    private bool _canMove = false, _isAppear = false;
    private PoolManager _poolManager;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private Transform _target;
    private Knockback _knockBack;
    private HealthManager _healthManager;
    [SerializeField] private List<Component> _componentsToDisable; 

    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _healthManager = GetComponent<HealthManager>();
        _knockBack = GetComponent<Knockback>();

        _poolManager = FindAnyObjectByType<PoolManager>();

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
    }
    void Start()
    {
        if(!_isAppear) StartCoroutine(DisableMovementForTime(_appearDelay));
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
        StartCoroutine(DisableMovementForTime(_appearDelay));
    }
    void OnDisable()
    {
        EnableMovement();
    }
    public void DisableMovement()
    {
        _canMove = false;
        _navMeshAgent.speed = 0;
    }

    public void EnableMovement()
    {
        _canMove = true;
        if(_navMeshAgent != null)
            _navMeshAgent.speed = _enemySO._speed;
    }
    private void ReturnInPool()
    {
        _poolManager.ReturnObject(gameObject, _enemySO._name);
    }
    private IEnumerator DisableMovementForTime(float _disableTime)
    {
        _isAppear = true;
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
}
