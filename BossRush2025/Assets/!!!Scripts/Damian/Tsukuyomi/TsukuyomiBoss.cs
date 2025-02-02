using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
    private NeedlessAttack _needlesAttack;
    public static Action<string> _TsukuyomiGravityPull;

    public static Action<string> _tsukuyomiLunarDiskAttack;
    private LunarDiskAttack _lunarDiskAttack;

    private Coroutine _inScriptAttack;

    //for triggering the string Actions from another script: TsukuyomiBoss._tsukuyomiLunarDiskAttack?.Invoke("Start or Pause or Resume or Stop");
    //for triggerig the string Actions from this script _tsukuyomiLunarDiskAttack?.Invoke("Start or Pause or Resume or Stop");
    private bool _canCrescent = true;
    private bool _canGravity = true;
    private int _currentAttackIndex;

    private bool _canNeedles = true;
    private bool _canLunarDisk = true;

    [SerializeField] float _attackDelay;
    [SerializeField] string _destroyParticles;

    private Collider2D _collider;
    private Flash _flash;

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
        if (_finishedArcAttack && _teleport._finishedAttack)
        {
            FlipToPlayer();
        }
    }
    protected override void HandleRunning() { }

    protected override void Initialize()
    {
        _swordAttacks = GetComponent<SwordStrikeAttack>();
        _teleport = GetComponent<Teleport>();
        _allRoundFire = GetComponent<AllRoundFire>();
        _needlesAttack = GetComponent<NeedlessAttack>();
        _lunarDiskAttack = GetComponent<LunarDiskAttack>();
        _collider = GetComponent<Collider2D>();

        _healthManager._onHit += CheckPhaseTransition;
        _gameManager.StartFight += StartGame;
        _gameManager.RitualFinished += FinishRitual;
        _gameManager.PlayerDie += StopAttack;
        _healthManager._onDie += _gameManager.DefeatedBoss;
        _healthManager._onDie += BossDie;
        _flash = GetComponentInChildren<Flash>();
        ChangeState(State.Idle);
    }
    protected override IEnumerator StartAttacks()
    {
        _animator.SetBool(_stateAnim, false);

        ChangeState(State.Idle);
        yield return new WaitForSeconds(1.3f);
        ChangeState(State.Walking);
        _collider.enabled = true;
        int attackTimes = UnityEngine.Random.Range(3 + _phase, 5 + _phase);
        int attackedTimes = 0;
        while (attackedTimes < attackTimes)
        {
            int randomAttack = UnityEngine.Random.Range(1, 5);
            if (_lastAttack == randomAttack) randomAttack = UnityEngine.Random.Range(1, 5);
            if(randomAttack == 4 && !_canLunarDisk)
            {
                randomAttack = UnityEngine.Random.Range(1, 4);
                if (_lastAttack == randomAttack) randomAttack = UnityEngine.Random.Range(1, 4);
            }
            _lastAttack = randomAttack;
            _currentAttackIndex = randomAttack;

            SignToNextAttack(randomAttack - 1, true);
            yield return new WaitForSeconds(1f);
            DisableMovement();
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
                    FlashSphere();
                    TsukuyomiAllRoundFire?.Invoke();
                    yield return new WaitUntil(() => _allRoundFire._finishedAttack);
                    break;
                case 4:
                    TestLunarDiskStart();
                    StartCoroutine(NoLunarDisk());
                    yield return new WaitForSeconds(0.8f);
                    break;
                case 5:
                    _TsukuyomiGravityPull?.Invoke("Start");
                    break;
            }

            attackedTimes++;
            EnableMovement();
            yield return new WaitForSeconds(_attackDelay);
        }
        ChangeState(State.Idle);
        _navMeshAgent.SetDestination(_gameManager.RitualCenter.position);
        _isRitual = true;
        yield return new WaitUntil(() => _navMeshAgent.remainingDistance < 0.3f && !_navMeshAgent.pathPending);
        transform.position = _gameManager.RitualCenter.position;
        _animator.SetBool(_stateAnim, true);
        _gameManager.RitualBegin();
        _finishedArcAttack = true;
        yield return new WaitForSeconds(1f);
        while (_isRitual)
        {
            int randomAttack = UnityEngine.Random.Range(1, 4);
            if(_lastAttack == randomAttack) randomAttack = UnityEngine.Random.Range(1, 4);
            if (!_canNeedles && randomAttack == 3)
            {
                randomAttack = UnityEngine.Random.Range(1, 3);
                if (_lastAttack == randomAttack) randomAttack = UnityEngine.Random.Range(1, 3);
            }
            _lastAttack = randomAttack;
            _currentAttackIndex = randomAttack + 4;

            SignToNextAttack(randomAttack - 1, false);
            yield return new WaitForSeconds(1f);

            switch (randomAttack)
            {
                case 1:
                    _inScriptAttack = StartCoroutine(ArcShooting(_arcProjectileCount));
                    yield return new WaitUntil(() => _finishedArcAttack);
                    break;
                case 2:
                    TsukuyomiAllRoundFire?.Invoke();
                    FlashSphere();
                    yield return new WaitForSeconds(_allRoundFire.GetAttackTime());
                    break;
                case 3:
                    _tsukuyomiNeedlesAttack?.Invoke();
                    yield return new WaitUntil(() => _needlesAttack._finishedAttack);
                    break;
                case 4:
                    _tsukuyomiCrescentAttack?.Invoke();
                    StartCoroutine(NoCrescent());
                    break;
            }
            yield return new WaitForSeconds(_attackDelay);
        }
    }
    private IEnumerator ArcShooting(int currentProjectileCount)
    {
        FlashSphere();
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
        StopCurrentAttack();
        ChangeState(State.Idle);
    }
    private void FinishRitual()
    {
        _isRitual = false;
        StopCurrentAttack();
        _attackCycle = StartCoroutine(StartAttacks());
    }
    private void BossDie()
    {
        AudioManager._instance.StopBGM();
        StopAttack();
        _collider.enabled = false;
        StartCoroutine(DeathAnim());
    }
    private IEnumerator DeathAnim()
    {

        yield return new WaitForSeconds(2f);

        AudioManager._instance.PlaySFX("Tsukuyomi death");
        float index = 1f;
        for (float i = 1f; i >= 0f; i -= 0.05f)
        {
            transform.DOScaleX(i * index, 0.1f * Mathf.Abs(i));
            index *= -1f;
            yield return new WaitForSeconds(0.1f * Mathf.Abs(i));
        }
        transform.localScale = Vector3.zero;
        GameObject destroyParticles = PoolManager._instance.GetObject(_destroyParticles);
        destroyParticles.transform.position = transform.position;
        AudioManager._instance.PlayBGM("Victory");
        this.enabled = false;
    }
    void OnDestroy()
    {
        _healthManager._onHit -= CheckPhaseTransition;
        _gameManager.StartFight -= StartGame;
        _gameManager.RitualFinished -= FinishRitual;
        _gameManager.PlayerDie -= StopAttack;
        _healthManager._onDie -= _gameManager.DefeatedBoss;
        _healthManager._onDie -= BossDie;
    }

    #region NoAttackCoroutines
    private IEnumerator NoNeedles()
    {
        _canNeedles = false;
        yield return new WaitUntil(() => _needlesAttack._finishedAttack);
        _canNeedles = true;
    }
    private IEnumerator NoLunarDisk()
    {
        _canLunarDisk = false;
        yield return new WaitForSeconds(_lunarDiskAttack.diskLifetime + 0.8f);
        _canLunarDisk = true;
    }
    private IEnumerator NoCrescent()
    {
        _canCrescent = false;
        yield return new WaitForSeconds(7f);
        _canCrescent = true;
    }
    private void StopCurrentAttack()
    {
        switch(_currentAttackIndex)
        {
            case 1:
                _swordAttacks.StopAttack();
                break;
            case 2:
                _teleport.StopTeleport();
                break;
            case 3:
                _allRoundFire.StopAttack();
                break;
            case 4:
                TestLunarDiskStop();
                break;
            case 5:
                if (_inScriptAttack != null)
                {
                    StopCoroutine(_inScriptAttack);
                    _inScriptAttack = null;
                }
                _finishedArcAttack = true;
                break;
            case 6:
                _allRoundFire.StopAttack();
                break;
            case 7:
                _needlesAttack.StopAttack();
                break;
        }
    }
    #endregion

    private void CheckPhaseTransition(float healthPart)
    {
        if (_healthManager.GetHealth() / _healthManager._maxHealth <= 0.5f && _phase < 2)
        {
            _phase = 2;
            _animator.SetInteger(_phaseAnim, 2);

            _swordAttacks.followSpeed += 2.5f;
            _swordAttacks.repeatTimes += 1;

            _teleport.repeatTimes += 1;
            _teleport.teleportDuration -= 0.2f;
            _teleport.projectileCount += 4;
            _teleport.attackInterval -= 0.5f;

            _allRoundFire.rows += 1;
            _allRoundFire.rowDelay += 0.2f;
            _allRoundFire.projectileSpeed += 1;

            _needlesAttack.delay -= 0.5f;
            _needlesAttack.countOfProjectiles += 2;

            _lunarDiskAttack.repeatTimes++;

            GetComponentInChildren<Light2D>().color = Color.white;

            StartCoroutine(PhaseTranslate());
        }
    }
    private IEnumerator PhaseTranslate()
    {
        _collider.enabled = false;
        _animator.SetBool(_stateAnim, false);
        if (_attackCycle != null)
        {
            StopCoroutine(_attackCycle);
            _attackCycle = null;
        }
        StopCurrentAttack();
        ChangeState(State.Idle);

        EnableMovement();

        _audioManager.StopBGM();
        StartCoroutine(BossRoar());
        yield return new WaitForSeconds(2f);

        _collider.enabled = true;
        _audioManager.PlayBGM(_secondTrack);
        _attackCycle = StartCoroutine(StartAttacks());
    }
    private void FlashSphere()
    {
        _audioManager.PlaySFX("Tsukuyomi Casting");
        _flash.LightOn(3, 0.2f, 0.8f);
    }
    #region LunarDisk
    [ContextMenu("Lunar Disk - Start Attack")]
    private void TestLunarDiskStart()
    {
        _tsukuyomiLunarDiskAttack?.Invoke("Start");
    }

    [ContextMenu("Lunar Disk - Pause Attack")]
    private void TestLunarDiskPause()
    {
        _tsukuyomiLunarDiskAttack?.Invoke("Pause");
    }

    [ContextMenu("Lunar Disk - Resume Attack")]
    private void TestLunarDiskResume()
    {
        _tsukuyomiLunarDiskAttack?.Invoke("Resume");
    }

    [ContextMenu("Lunar Disk - Stop Attack")]
    private void TestLunarDiskStop()
    {
        _tsukuyomiLunarDiskAttack?.Invoke("Stop");
    }
    #endregion
}
