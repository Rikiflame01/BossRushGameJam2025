using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TsukuyomiBoss : EntityState
{
    public static event Action TsukuyomiTeleport;
    private Teleport _teleport;

    public static event Action TsukuyomiAllRoundFire;
    private AllRoundFire _allRoundFire;

    public static event Action _tsukuyomiCrescentAttack;

    public static event Action _tsukuyomiSwordStrikeAttack;
    private SwordStrikeAttack _swordAttacks;

    public static event Action _tsukuyomiNeedlesAttack;

     public static event Action<string> _TsukuyomiGravityPull;

    [SerializeField] float _attackDelay;

    [Header("Arc Shooting")]
    [SerializeField] private int _arcProjectileCount = 5;
    [SerializeField] private float _arcDelay = 0.2f;
    [SerializeField] private float _arcAngle = 40f;
    [SerializeField] private float _arcWaveDelay = 0.7f;
    [SerializeField] private string _arcProjectile;

    private bool _finishedArcAttack = true;
    private int _lastAttack = -1;

    protected override void HandleIdle() { }
    protected override void HandleWalking()
    {
        if (!(_navMeshAgent.pathPending || _moveWaiting || _navMeshAgent.remainingDistance != 0)) StartCoroutine(MoveWaiting());
        if (_finishedArcAttack)
        {
            FlipToPlayer();
        }
    }
    protected override void HandleRunning() {  }
    
    protected override void Initialize()
    {
        _swordAttacks = GetComponent<SwordStrikeAttack>();
        _teleport = GetComponent<Teleport>();
        _allRoundFire = GetComponent<AllRoundFire>();

        _healthManager._onHit += CheckPhaseTransition;
        _gameManager.StartFight += StartGame;
        _gameManager.RitualFinished += FinishRitual;
        _gameManager.PlayerDie += StopAttack;
        _healthManager._onDie += _gameManager.DefeatedBoss;
        _healthManager._onDie += BossDie;

        ChangeState(State.Idle);
    }
    private void CheckPhaseTransition(float damage)
    {

    }
    private void FinishRitual()
    {

    }
    protected override IEnumerator StartAttacks()
    {
        _animator.SetBool(_stateAnim, false);

        ChangeState(State.Idle);
        yield return new WaitForSeconds(1.3f);
        ChangeState(State.Walking);
        int attackTimes = UnityEngine.Random.Range(3 + _phase, 5 + _phase);
        int attackedTimes = 0;
        while (attackedTimes < attackTimes)
        {
            int randomAttack;
            randomAttack = UnityEngine.Random.Range(1, 5);
            if (randomAttack == _lastAttack) randomAttack = UnityEngine.Random.Range(1, 5);
            _lastAttack = randomAttack;

            SignToNextAttack(randomAttack - 1, true);
            yield return new WaitForSeconds(1f);

            switch (randomAttack)
            {
                case 1:
                    _tsukuyomiSwordStrikeAttack?.Invoke();
                    yield return new WaitUntil(() => _swordAttacks._finishedAttack);
                    break;

                case 2:
                    TsukuyomiTeleport?.Invoke();
                    yield return new WaitUntil(() => _teleport._finishedAttack);
                    break;

                case 3:
                    TsukuyomiAllRoundFire?.Invoke();
                    yield return new WaitUntil(() => _allRoundFire._finishedAttack);
                    break;

                case 4:
                    _TsukuyomiGravityPull?.Invoke("Start");
                    break;
            }

            attackedTimes++;
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
            int randomAttack = UnityEngine.Random.Range(1, 4);

            SignToNextAttack(randomAttack - 1, false);
            yield return new WaitForSeconds(1f);

            switch (randomAttack)
            {
                case 1:
                    StartCoroutine(ArcShooting(_arcProjectileCount));
                    yield return new WaitUntil(() => _finishedArcAttack);
                    break;
                case 2:
                    TsukuyomiAllRoundFire?.Invoke();
                    yield return new WaitForSeconds(0.6f);
                    break;
                case 3:
                    _tsukuyomiNeedlesAttack?.Invoke();
                    break;
                case 4:
                    _tsukuyomiCrescentAttack?.Invoke();
                    break;
            }
            yield return new WaitForSeconds(_attackDelay);
        }
    }
    private IEnumerator ArcShooting(int currentProjectileCount)
    {
        _finishedArcAttack = false;
        for (int j = 0; j < 3; j++)
        {
            Vector2 direction = _player.transform.position - transform.position;
            float startAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float angleCof = 0f;
            float currentAttackAngle = UnityEngine.Random.Range(0, 2) == 0 ? _arcAngle : -_arcAngle;
            if (currentProjectileCount % 2 == 0)
            {
                angleCof = 0.5f;
            }
            int board = currentProjectileCount / 2;

            for (int i = -board; i < board + currentProjectileCount % 2; i++)
            {
                GameObject currentProjectile = _poolManager.GetObject(_arcProjectile);
                currentProjectile.transform.position = transform.position;
                float currentAngle = startAngle + (i + angleCof) * currentAttackAngle;
                if (currentAngle > 360f) currentAngle -= 360f;
                else if (currentAngle < 0f) currentAngle += 360f;
                currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, currentAngle);
                yield return new WaitForSeconds(_arcDelay);
            }
            yield return new WaitForSeconds(_arcWaveDelay);
        }
        _finishedArcAttack = true;
    }
    private void StopAttack()
    {
        if (_attackCycle != null)
        {
            StopCoroutine(_attackCycle);
            _attackCycle = null;
        }
        ChangeState(State.Idle);
    }
    private void BossDie()
    {
        _bossHealth.DOScale(0f, 0.35f);
    }
    void OnDestroy()
    {
        
    }
}
