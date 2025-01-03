using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemySO _enemySO;
    [SerializeField] private MeleeDamage _meleeDamage;
    private bool _canMove = true;
    private NavMeshAgent _navMeshAgent;
    private Transform _target;
    private Knockback _knockBack;
    private HealthManager _healthManager;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _healthManager = GetComponent<HealthManager>();
        _knockBack = GetComponent<Knockback>();

        _knockBack._onStartKnockback += DisableMovement;
        _knockBack._onFinishKnockback += EnableMovement;

        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _navMeshAgent.speed = _enemySO._speed;

        _healthManager.SetHealth(_enemySO._health);
        _meleeDamage.SetDamage(_enemySO._damage);
        _meleeDamage.SetAttackInterval(_enemySO._attackInterval);
        
        _target = GameObject.FindAnyObjectByType<Movement>().transform;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (!_canMove)
            return;

        _navMeshAgent.SetDestination(_target.position);
    }

    public void DisableMovement()
    {
        _canMove = false;
        _navMeshAgent.speed = 0;
    }

    public void EnableMovement()
    {
        _canMove = true;
        _navMeshAgent.speed = _enemySO._speed;
    }
}
