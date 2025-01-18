using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemySO _enemySO;
    [SerializeField] private MeleeDamage _meleeDamage;
    private bool _canMove = false;
    private PoolManager _poolManager;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private Transform _target;
    private Knockback _knockBack;
    private HealthManager _healthManager;

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
}
