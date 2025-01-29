/*
 * AUthor:Thorin
 * Description: sample boss that inherits the entity state abstract class
 * intention implement custom logic here.
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class Susano : EntityState
{
    [SerializeField] private List<Component> _componentsToDisableOnDisappear;

    [SerializeField] private float _attackDelay;

    private Coroutine _currentAttack;

    private Rigidbody2D _rb;

    private int _lastAttack = -1;


    [Header("Melee Attack")]
    [SerializeField] private GameObject _splash;
    [SerializeField] private float _meleeAttackDelay = 1f;

    [SerializeField] private float _circualSwordStrikeAttackRadius = 5f;
    [SerializeField] private float _circualSwordStrikeAttackDamage = 6f;

    [Space]
    [SerializeField] private float _jumpAttackAppearNearRadius = 6f;
    [SerializeField] private float _jumpAttackRadius = 3f;
    [SerializeField] private float _jumpAttackDamage = 6f;
    [SerializeField] private string _jumpShadow;

    [Header("Shooting")]
    [SerializeField] private float _chargeTime;
    [SerializeField] private float _angleBetweenProjectiles;
    [SerializeField] private float _projectileCount;
    [SerializeField] private string _shootProjectile;

    [Header("Acceleration Attack")]
    [SerializeField] private float _accelerationSpeed = 15f;
    [SerializeField] private float _accelerationAttack = 10f;
    [SerializeField] private float _accelerationAttackTime = 5f;
    private bool _isAccelerationAttack = false;
    private Vector2 _accelerationDirection;
    private int _accelerationAnim = Animator.StringToHash("acceleration");

    [Header("Summon Enemies")]
    [SerializeField] private int _minEnemyCount = 2;
    [SerializeField] private int _maxEnemyCount = 4;
    [SerializeField] private string _enemy;
    [SerializeField] private float _summonDelay = 0.3f;
    [SerializeField] private string _enemyAppearParticle;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    [SerializeField] private bool _finishedCoroutine = true;

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
    [SerializeField] private float _axisDelay;
    private bool _canAxisProjectile = true;

    public static event System.Action _sussanoWaveAttack;
    private WaveAttack _waveAttack;
    private bool _canWaveAttack = true;

    private Flash _sphereFlash;
    [SerializeField] private GameObject _deathBossPrefab;

    protected override void HandleIdle() { }
    protected override void HandleWalking() 
    { 
        if (!(_navMeshAgent.pathPending || _moveWaiting || _navMeshAgent.remainingDistance != 0)) StartCoroutine(MoveWaiting()); 
        if(_finishedCoroutine)
        {
            FlipToPlayer();
        }
    }
    protected override void HandleRunning() { }

    protected override void Initialize()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        
        _sphereFlash = GetComponentInChildren<Flash>();

        _waveAttack = GetComponent<WaveAttack>();
        
        _healthManager._onHit += CheckPhaseTransition;
        _gameManager.StartFight += StartGame;
        _gameManager.RitualFinished += FinishRitual;
        _gameManager.PlayerDie += StopAttack;
        _healthManager._onDie += _gameManager.DefeatedBoss;
        _healthManager._onDie += BossDie;

        ChangeState(State.Idle);
        _collider.enabled = false;
    }

    protected override IEnumerator StartAttacks()
    {
        _animator.SetBool(_stateAnim, false);

        _collider.enabled = true;
        _collider.isTrigger = false;

        ChangeState(State.Idle);
        yield return new WaitForSeconds(1.3f);
        ChangeState(State.Walking);
        int attackTimes = Random.Range(3 + _phase, 5 + _phase);
        int attackedTimes = 0;
        while (attackedTimes < attackTimes)
        {
            int randomAttack;
            if (attackedTimes + 1 == attackTimes)
            {
                randomAttack = Random.Range(1, 4);
            }
            else
            {
                if (_phase > 1 && _canWaveAttack && Random.Range(0, 2) == 0)
                {
                    randomAttack = 5;
                }
                else
                {
                    randomAttack = Random.Range(1, 5);
                    if (randomAttack == _lastAttack) randomAttack = Random.Range(1, 5);
                    _lastAttack = randomAttack;

                }
            }

            SignToNextAttack(randomAttack - 1, true);
            yield return new WaitForSeconds(1f);

            switch (randomAttack)
            {
                case 1:
                    _currentAttack = StartCoroutine(StartMeleeAttack());
                    break;
                case 2:
                    _currentAttack = StartCoroutine(Shooting());
                    break;

                case 3:
                    _currentAttack = StartCoroutine(AccelerationAttack());
                    break;

                case 4:
                    _currentAttack = StartCoroutine(SpawnEnemiesAttack());
                    StartCoroutine(DisableMovementForTime(1f));
                    break;
                case 5:
                    _sussanoWaveAttack?.Invoke();
                    StartCoroutine(NoWaveAttack());
                    break;
            }

            attackedTimes++;

            if (!_finishedCoroutine)
                yield return new WaitUntil(() => _finishedCoroutine);
            yield return new WaitForSeconds(_attackDelay);
        }
        ChangeState(State.Idle);
        _navMeshAgent.SetDestination(_gameManager.RitualCenter.position);
        _isRitual = true;
        yield return new WaitUntil(() => _navMeshAgent.remainingDistance < 0.1f && !_navMeshAgent.pathPending);
        transform.position = _gameManager.RitualCenter.position;
        _animator.SetBool(_stateAnim, true);
        _gameManager.RitualBegin();
        yield return new WaitForSeconds(1f);
        while (_isRitual)
        {
            int randomAttack = Random.Range(1, 4);
            if (randomAttack == 3 && !_canAxisProjectile)
                randomAttack = Random.Range(1, 3);

            SignToNextAttack(randomAttack - 1, false);
            yield return new WaitForSeconds(1f);

            switch (randomAttack)
            {
                case 1:
                    _currentAttack = StartCoroutine(RandomShooting());
                    break;

                case 2:
                    _currentAttack = StartCoroutine(ArcShooting(_arcProjectileCount));
                    break;

                case 3:
                    AxisShooting();
                    StartCoroutine(NoAxisAttack());
                    break;
            }

            if (!_finishedCoroutine)
                yield return new WaitUntil(() => _finishedCoroutine);
            yield return new WaitForSeconds(_attackDelay);
        }
    }
    #region Attacks
    private IEnumerator AccelerationAttack()
    {
        _finishedCoroutine = false;
        _isAccelerationAttack = true;
        _animator.SetBool(_accelerationAnim, true);

        DisableMovement();

        yield return new WaitForSeconds(0.8f);

        Vector2 moveDirection = -(transform.position - _player.transform.position).normalized;
        _accelerationDirection = moveDirection;
        _rb.constraints = RigidbodyConstraints2D.None;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb.linearVelocity = moveDirection * _accelerationSpeed;

        yield return new WaitForSeconds(_accelerationAttackTime);

        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _rb.linearVelocity = Vector2.zero;

        EnableMovement();

        _animator.SetBool(_accelerationAnim, false);
        _isAccelerationAttack = false;
        yield return new WaitForSeconds(0.5f);
        _finishedCoroutine = true;
    }

    private IEnumerator SpawnEnemiesAttack()
    {
        FlashSphere();
        int enemiesCounts = Random.Range(_minEnemyCount, _maxEnemyCount + 1);
        for (int i = 0; i < enemiesCounts; i++)
        {
            yield return new WaitForSeconds(_summonDelay);
            GameObject spawnedEnemy = _poolManager.GetObject(_enemy);
            Vector2 randomPlace = Random.insideUnitCircle * 9f;
            float distanceToBoss = Vector2.Distance(transform.position, randomPlace);
            if (distanceToBoss < 3f)
            {
                randomPlace = (randomPlace - (Vector2)transform.position).normalized * (3 - distanceToBoss);
            }
            spawnedEnemy.transform.position = randomPlace;
            _poolManager.GetObject(_enemyAppearParticle).transform.position = randomPlace;
        }
    }

    private IEnumerator StartMeleeAttack()
    {
        _finishedCoroutine = false;
        DisableMovement();
        int repeatTimes = 3;
        bool lastCircualSwordStrike = true;
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < repeatTimes; i++)
        {
            int randomAttack = Random.Range(0, 2);
            if (randomAttack == 0 || randomAttack == 1 && lastCircualSwordStrike)
            {
                lastCircualSwordStrike = false;

                Disappear();

                Vector2 playerPosition = _player.position;
                Vector2 bossUpPosition = new Vector2(0, _jumpAttackAppearNearRadius);
                transform.position = playerPosition + bossUpPosition;

                float bossDashPositionY = playerPosition.y;
                _poolManager.GetObject(_jumpShadow).transform.position = playerPosition;
                yield return new WaitForSeconds(1f);
                FlipToPlayer();
                Appear();

                _collider.isTrigger = true;
                transform.DOMoveY(bossDashPositionY, 0.15f);
                yield return new WaitForSeconds(0.135f);

                GameObject currentSplash = Instantiate(_splash, playerPosition + Vector2.down * 2, Quaternion.identity);
                currentSplash.transform.localScale = Vector3.one * 1.5f;
                Destroy(currentSplash, 1f);

                yield return new WaitForSeconds(0.15f);
                AreaDamage(_jumpAttackRadius, _jumpAttackDamage, transform.position);

                _collider.isTrigger = false;
            }
            else if (randomAttack == 1)
            {
                lastCircualSwordStrike = true;
                GameObject currentSplash = Instantiate(_splash, (Vector2)transform.position + Vector2.down * 2.7f, Quaternion.identity);
                currentSplash.transform.localScale = Vector3.one * 3f;
                yield return new WaitForSeconds(0.15f);
                AreaDamage(_circualSwordStrikeAttackRadius, _circualSwordStrikeAttackDamage, transform.position);
            }

            yield return new WaitForSeconds(_meleeAttackDelay);
        }
        EnableMovement();
        _finishedCoroutine = true;
    }

    private IEnumerator Shooting()
    {
        _finishedCoroutine = false;
        FlashSphere();
        DisableMovement();
        yield return new WaitForSeconds(_chargeTime);
        Vector2 direction = _player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        int helpInt = 1;
        for (int i = 0; i < _projectileCount; i++)
        {
            angle += i * helpInt * _angleBetweenProjectiles;
            GameObject currentProjectile = _poolManager.GetObject(_shootProjectile);
            currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, angle);
            currentProjectile.transform.position = transform.position;
            helpInt *= -1;
        }
        StartCoroutine(DisableMovementForTime(0.5f));
        _finishedCoroutine = true;
    }
    private IEnumerator RandomShooting()
    {
        _finishedCoroutine = false;
        for (int i = 0; i < _projectileCount; i++)
        {
            GameObject currentProjectile = _poolManager.GetObject(_randomShootProjectile);
            currentProjectile.transform.position = transform.position;
            currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
            yield return new WaitForSeconds(_randomDelay);
        }
        _finishedCoroutine = true;
    }
    private IEnumerator ArcShooting(int currentProjectileCount)
    {
        _finishedCoroutine = false;
        Vector2 direction = _player.transform.position - transform.position;
        float startAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angleCof = 0f;
        float currentAttackAngle = Random.Range(0, 2) == 0 ? _arcAngle : -_arcAngle;
        if (currentProjectileCount % 2 == 0)
        {
            angleCof = 0.5f;
        }
        int board = currentProjectileCount / 2;

        for (int i = -board; i < board + currentProjectileCount % 2; i++)
        {
            GameObject currentProjectile = _poolManager.GetObject(_randomShootProjectile);
            currentProjectile.transform.position = transform.position;
            float currentAngle = startAngle + (i + angleCof) * currentAttackAngle;
            if (currentAngle > 360f) currentAngle -= 360f;
            else if (currentAngle < 0f) currentAngle += 360f;
            currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, currentAngle);
            yield return new WaitForSeconds(_arcDelay);
        }
        _finishedCoroutine = true;
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
    private IEnumerator NoAxisAttack()
    {
        _canAxisProjectile = false;
        yield return new WaitForSeconds(_axisDelay);
        _canAxisProjectile = true;
    }
    private IEnumerator NoWaveAttack()
    {
        _canWaveAttack = false;
        yield return new WaitForSeconds(_waveAttack.GetAttackTime());
        _canWaveAttack = true;
    }
    #endregion

    private void CheckPhaseTransition(float healthPart)
    {
        if (_healthManager.GetHealth() / _healthManager._maxHealth <= 0.6f && _phase < 2)
        {
            _phase = 2;
            _animator.SetInteger(_phaseAnim, 2);

            _projectileCount += 2;
            _minEnemyCount += 2;
            _maxEnemyCount += 2;
            _meleeAttackDelay -= 0.3f;
            _navMeshAgent.speed = _speed + 2;

            _randomDelay -= 0.2f;
            _axisDelay = 2f;
            _arcProjectileCount += 4;
            StartCoroutine(PhaseTranslate());
        }
    }
    private void FinishRitual()
    {
        _isRitual = false;
        _finishedCoroutine = true;
        StopCoroutine(_currentAttack);
        _currentAttack = null;
        _attackCycle = StartCoroutine(StartAttacks());
    }
    private IEnumerator PhaseTranslate()
    {
        _collider.enabled = false;
        _animator.SetBool(_stateAnim, false);
        _animator.SetBool(_accelerationAnim, false);
        StopCoroutine(_attackCycle);
        if (_currentAttack != null)
        {
            StopCoroutine(_currentAttack);
            _currentAttack = null;
        }
        _finishedCoroutine = true;

        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _rb.linearVelocity = Vector2.zero;
        EnableMovement();
        _animator.SetBool(_accelerationAnim, false);
        _isAccelerationAttack = false;

        _audioManager.StopBGM();
        StartCoroutine(BossRoar());
        yield return new WaitForSeconds(2f);

        _collider.enabled = true;
        _audioManager.PlayBGM(_secondTrack);

        _attackCycle = StartCoroutine(StartAttacks());
    }

    private void StopAttack()
    {
        if (_currentAttack != null)
        {
            StopCoroutine(_currentAttack);
            _currentAttack = null;
        }
        if (_attackCycle != null)
        {
            StopCoroutine(_attackCycle);
            _attackCycle = null;
        }
        ChangeState(State.Idle);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _jumpAttackRadius);
        Gizmos.DrawWireSphere(transform.position, _circualSwordStrikeAttackRadius);
    }
    private void FlashSphere()
    {
        _sphereFlash.LightOn(3, 0.2f, 0.8f);
    }
    private void BossDie()
    {
        _audioManager.StopBGM();
        _collider.enabled = false;
        _healthManager._onDie -= BossDie;
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _rb.linearVelocity = Vector2.zero;
        StopAttack();
        DisableMovement();
        ChangeState(State.Idle);
        Instantiate(_deathBossPrefab, transform.position, Quaternion.identity);
        _spriteRenderer.enabled = false;
        StartCoroutine(LastCameraShake());
        _bossHealth.DOScale(0, 0.35f);
    }
    private IEnumerator LastCameraShake()
    {
        yield return new WaitForSeconds(3f);
        CameraShake._instance.Shake(1f, 1.9f);
        this.enabled = false;
    }
}