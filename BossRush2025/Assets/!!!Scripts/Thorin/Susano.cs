/*
 * AUthor:Thorin
 * Description: sample boss that inherits the entity state abstract class
 * intention implement custom logic here.
 */
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class Susano : EntityState
{
    [SerializeField] private List<Component> _componentsToDisableOnDisappear;
    [SerializeField] private float _speed;
    [SerializeField] private float _maxMoveDistance;
    private NavMeshAgent _navMeshAgent;
    private bool _moveWaiting = false;

    private Transform _player;
    private PoolManager _poolManager;
    private GameManager _gameManager;
    private Rigidbody2D _rb;

    [Header("Acceleration Attck")]
    [SerializeField] private float _accelerationSpeed = 15f;
    [SerializeField] private float _accelerationAttack = 10f;
    [SerializeField] private float _accelerationAttackTime = 5f;
    private bool _isAccelerationAttack = false;
    private Vector2 _accelerationDirection;

    [Header("Shooting")]
    [SerializeField] private float _chargeTime;
    [SerializeField] private float _angleBetweenProjectiles;
    [SerializeField] private float _projectileCount;
    [SerializeField] private string _shootProjectile;

    [Header("Melee Attack")]
    [SerializeField] private float _circualSwordStrikeAttackRadius = 5f;
    [SerializeField] private float _circualSwordStrikeAttackDamage = 6f;

    [Space]
    [SerializeField] private float _jumpAttackAppearNearRadius = 6f;
    [SerializeField] private float _jumpAttackRadius = 3f;
    [SerializeField] private float _jumpAttackDamage = 6f;

    [Header("Summon Enemies")]
    [SerializeField] private GameObject _enemy;
    [SerializeField] private float _summonDelay = 0.3f;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private bool _finishedCoroutine = true;

    [Header("Random Shooting")]
    [SerializeField] private float _randomProjectileCount;
    [SerializeField] private float _randomDelay = 0.5f;

    [Header("Arc Shooting")]
    [SerializeField] private int _arcProjectileCount = 10;
    [SerializeField] private float _arcDelay = 0.2f;
    [SerializeField] private float _arcAngle = 10f;

    [SerializeField] private string _randomShootProjectile;

    [Header("Axis Projectile")]
    [SerializeField] private string _axisProjectile;

    protected override void HandleIdle() { Debug.Log("Enemy B Idle Behavior"); }
    protected override void HandleWalking() { if (!(_navMeshAgent.pathPending || _moveWaiting || _navMeshAgent.remainingDistance != 0)) StartCoroutine(MoveWaiting()); }
    protected override void HandleRunning() { Debug.Log("Enemy B Running Behavior"); }

    protected override void Initialize()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();

        _player = FindAnyObjectByType<Movement>().transform;
        _poolManager = FindAnyObjectByType<PoolManager>();
        _gameManager = FindAnyObjectByType<GameManager>();

        StartCoroutine(StartAttacks());
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _navMeshAgent.speed = _speed;

        ChangeState(State.Walking);
    }

    private IEnumerator StartAttacks()
    {
        yield return new WaitForSeconds(1f);
        int attackTimes = Random.Range(4, 8);
        int attackedTimes = 0;
        while (attackedTimes < attackTimes)
        {
            int randomAttack = Random.Range(1, 5);
            switch (randomAttack)
            {
                case 1:
                    StartCoroutine(StartMeleeAttack());
                    break;

                case 2:
                    StartCoroutine(SpawnEnemiesAttack());
                    StartCoroutine(DisableMovementForTime(1f));
                    break;

                case 3:
                    StartCoroutine(Shooting());
                    break;

                case 4:
                    StartCoroutine(AccelerationAttack());
                    break;
            }

            attackedTimes++;

            if (!_finishedCoroutine)
                yield return new WaitUntil(() => _finishedCoroutine);
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator AccelerationAttack()
    {
        _finishedCoroutine = false;
        _isAccelerationAttack = true;

        DisableMovement();

        Vector2 moveDirection = -(transform.position - _player.transform.position).normalized;
        _accelerationDirection = moveDirection;
        _rb.constraints = RigidbodyConstraints2D.None;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb.linearVelocity = moveDirection * _accelerationSpeed;

        yield return new WaitForSeconds(_accelerationAttackTime);

        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _rb.linearVelocity = Vector2.zero;

        EnableMovement();

        _isAccelerationAttack = false;
        _finishedCoroutine = true;
    }

    private IEnumerator SpawnEnemiesAttack()
    {
        int enemiesCounts = Random.Range(5, 10);
        for (int i = 0; i < enemiesCounts; i++)
        {
            yield return new WaitForSeconds(_summonDelay);
            GameObject spawnedEnemy = Instantiate(_enemy, Random.insideUnitCircle * 9f, Quaternion.identity);
        }
    }

    private IEnumerator StartMeleeAttack()
    {
        Debug.Log("Start Melee Attack");
        _finishedCoroutine = false;
        DisableMovement();
        int repeatTimes = 5;
        bool lastCircualSwordStrike = false;
        for (int i = 0; i < repeatTimes; i++)
        {
            int randomAttack = Random.Range(0, 2);
            if (randomAttack == 0 || randomAttack == 1 && lastCircualSwordStrike)
            {
                lastCircualSwordStrike = false;

                Disappear();
                yield return new WaitForSeconds(1.0f);
                Vector2 playerPosition = _playerMovement.transform.position;
                Vector2 bossUpPosition = new Vector2(0, _jumpAttackAppearNearRadius);
                transform.position = playerPosition + bossUpPosition;
                Appear();

                float bossDashPositionY = playerPosition.y;
                float randomDelay = Random.Range(1.0f, 2.0f);

                yield return new WaitForSeconds(randomDelay);

                _collider.isTrigger = true;
                transform.DOMoveY(bossDashPositionY, 0.15f);
                yield return new WaitForSeconds(0.15f);
                AreaDamage(_jumpAttackRadius, _jumpAttackDamage, transform.position);

                _collider.isTrigger = false;
            }
            else if (randomAttack == 1)
            {
                lastCircualSwordStrike = true;

                AreaDamage(_circualSwordStrikeAttackRadius, _circualSwordStrikeAttackDamage, transform.position);
            }

            yield return new WaitForSeconds(1);
        }
        EnableMovement();
        _finishedCoroutine = true;
    }

    private IEnumerator Shooting()
    {
        DisableMovement();
        yield return new WaitForSeconds(_chargeTime);
        Vector2 direction = _player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        int helpInt = 1;
        for (int i = 0; i < _projectileCount; i++)
        {
            angle += i * helpInt * _angleBetweenProjectiles;
            GameObject currentProjectile = _poolManager.GetObject(_shootProjectile);
            currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, angle + i * helpInt * _angleBetweenProjectiles);
            currentProjectile.transform.position = transform.position;
            helpInt *= -1;
        }
        StartCoroutine(DisableMovementForTime(1f));
    }
    private IEnumerator RandomShooting()
    {
        for (int i = 0; i < _projectileCount; i++)
        {
            GameObject currentProjectile = _poolManager.GetObject(_randomShootProjectile);
            currentProjectile.transform.position = transform.position;
            currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
            yield return new WaitForSeconds(_randomDelay);
        }
    }
    private IEnumerator ArcShooting(int currentProjectileCount)
    {
        Vector2 direction = _player.transform.position - transform.position;
        float startAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angleCof = 0f;
        if (currentProjectileCount % 2 == 0)
        {
            angleCof = 0.5f;
        }
        int board = currentProjectileCount / 2;
        for (int i = -board; i < board + currentProjectileCount % 2; i++)
        {
            GameObject currentProjectile = _poolManager.GetObject(_randomShootProjectile);
            currentProjectile.transform.position = transform.position;
            float currentAngle = startAngle + (i + angleCof) * _arcAngle;
            if (currentAngle > 360f) currentAngle -= 360f;
            else if (currentAngle < 0f) currentAngle += 360f;
            currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, currentAngle);
            yield return new WaitForSeconds(_arcDelay);
        }
    }
    private void AxisShooting()
    {
        GameObject currentProjectile = _poolManager.GetObject(_axisProjectile);
    }
    void Disappear()
    {
        _spriteRenderer.enabled = false;
        foreach (Behaviour component in _componentsToDisableOnDisappear)
        {
            component.enabled = false;
        }
    }

    void Appear()
    {
        _spriteRenderer.enabled = true;
        foreach (Behaviour component in _componentsToDisableOnDisappear)
        {
            component.enabled = true;
        }
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        if (_isAccelerationAttack)
        {
            Vector2 newDirection = Vector2.Reflect(_accelerationDirection, other.contacts[0].normal);
            if (other.collider.TryGetComponent<Movement>(out Movement playerMovement))
            {
                if (playerMovement.TryGetComponent<HealthManager>(out HealthManager healthManager))
                {
                    healthManager.TakeDamage(_accelerationAttack);
                }

                if (playerMovement.TryGetComponent<Knockback>(out Knockback knockBack))
                {
                    Vector3 knockBackDirection = Vector3.zero;
                    int randomKnockBackDirection = Random.Range(0, 2);

                    if (randomKnockBackDirection == 0)
                        knockBackDirection = new Vector3(0, 1f, 0);
                    else if (randomKnockBackDirection == 1)
                        knockBackDirection = new Vector3(0, -1f, 0);
                    
                    knockBack.PlayKnockBack(_player.position + knockBackDirection, 25f, 0.5f);
                }

                newDirection = _accelerationDirection;
                CameraShake._instance.Shake(1.0f, 0.25f);
            }

            _accelerationDirection = newDirection;
            _rb.linearVelocity = newDirection * _accelerationSpeed;
        }
    }

    /*void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Movement>(out Movement playerMovement))
        {
            if (playerMovement.TryGetComponent<HealthManager>(out HealthManager healthManager))
            {
                healthManager.TakeDamage(_jumpAttackDamage);
            }

            CameraShake._instance.Shake(1.0f, 0.25f);
        }
    }*/
    IEnumerator DisableMovementForTime(float duration)
    {
        DisableMovement();
        yield return new WaitForSeconds(duration);
        EnableMovement();
    }
    void DisableMovement()
    {
        _navMeshAgent.speed = 0f;
    }
    void EnableMovement()
    {
        _navMeshAgent.speed = _speed;
    }
    private void AreaDamage(float radius, float damage, Vector2 point)
    {
        List<RaycastHit2D> collidingObjects = Physics2D.CircleCastAll(point, radius, Vector2.zero).ToList();
        foreach (RaycastHit2D hit in collidingObjects)
        {
            if (hit.collider.transform.TryGetComponent<Movement>(out Movement playerMovement))
            {
                if (playerMovement.TryGetComponent<HealthManager>(out HealthManager healthManager))
                {
                    healthManager.TakeDamage(damage);
                }

                if (playerMovement.TryGetComponent<Knockback>(out Knockback knockBack))
                {
                    knockBack.PlayKnockBack(transform.position, 15f, 0.5f);
                }
                CameraShake._instance.Shake(0.5f, 0.5f);
                break;
            }
        }
    }
    private IEnumerator MoveWaiting()
    {
        _moveWaiting = true;
        yield return new WaitForSeconds(Random.Range(0.8f, 2f));
        MoveToRandomPoint();
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
                Debug.Log(3);
                break;
            }
        }
    }
    bool IsPointReachable(Vector2 point)
    {
        NavMeshHit hit;
        if(NavMesh.Raycast(transform.position, point, out hit, NavMesh.AllAreas))
        {
            if (hit.position != transform.position) return false;
        }
        return true;
    }
}