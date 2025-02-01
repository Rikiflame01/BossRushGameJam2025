using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

using DG.Tweening;

public class AmaterasuBoss : EntityState
{
    public static event Action TsukuyomiTeleport;
    private Teleport _teleport;
    public static event Action TsukuyomiAllRoundFire;
    private AllRoundFire _allRoundFire;
    public static event Action _tsukuyomiNeedlesAttack;
    private NeedlessAttack _needlesAttack;

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

    private int _attackAnim = Animator.StringToHash("attack");
    [SerializeField] private Light2D _sun;
    [SerializeField] float _attackDelay;
    [SerializeField] string _destroyParticles;

    private Collider2D _collider;
    private Flash _flash;

    [Header("Smiles")]
    [SerializeField] private GameObject _smiles;
    [SerializeField] private Vector2 _spawnSmilePos;
    [SerializeField] private float _smileDuration;
    private List<GameObject> _smileList = new List<GameObject>();

    [Header("Summon Enemies")]
    [SerializeField] private Vector2 _spawnPoints;
    [SerializeField] private string _enemy;
    [SerializeField] private float _summonDelay = 0.3f;
    [SerializeField] private string _enemyAppearParticle;

    [Header("Random Shooting")]
    [SerializeField] private float _randomProjectileCount;
    [SerializeField] private float _randomDelay = 0.5f;

    [Header("Arc Shooting")]
    [SerializeField] private int _arcProjectileCount = 5;
    [SerializeField] private float _arcDelay = 0.2f;
    [SerializeField] private float _arcAngle = 40f;
    [SerializeField] private float _arcWaveDelay = 0.7f;

    [SerializeField] private string _randomShootProjectile;

    [Header("Axis Projectile")]
    [SerializeField] private string _axisProjectile;
    [SerializeField] private float _axisDelay;
    [SerializeField] private int _axisCount;

    private bool _canAxisProjectile = true;
    public bool _finishedCoroutine = true;
    private int _lastAttack = -1;

    protected override void HandleIdle() { }
    protected override void HandleWalking()
    {
        if (!(_navMeshAgent.pathPending || _moveWaiting || _navMeshAgent.remainingDistance != 0)) StartCoroutine(MoveWaiting());
        if (_finishedCoroutine && _teleport._finishedAttack)
        {
            FlipToPlayer();
        }
    }
    protected override void HandleRunning() { }

    protected override void Initialize()
    {
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
            int randomAttack = UnityEngine.Random.Range(1, 6);
            if (_lastAttack == randomAttack) randomAttack = UnityEngine.Random.Range(1, 6);
            if (randomAttack == 4 && !_canLunarDisk)
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
                    SpawnTwoSmiles();
                    EnableMovement();
                    _animator.SetTrigger(_attackAnim);
                    yield return new WaitForSeconds(_smileDuration * 0.7f);
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
                    _animator.SetTrigger(_attackAnim);
                    TestLunarDiskStart();
                    StartCoroutine(NoLunarDisk());
                    yield return new WaitForSeconds(0.8f);
                    break;
                case 5:
                    _animator.SetTrigger(_attackAnim);
                    _inScriptAttack = StartCoroutine(SpawnEnemiesAttack());
                    yield return new WaitForSeconds(0.7f);
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
        yield return new WaitForSeconds(1f);
        _finishedCoroutine = true;
        while (_isRitual)
        {
            int randomAttack = UnityEngine.Random.Range(1, 5);
            if (_lastAttack == randomAttack) randomAttack = UnityEngine.Random.Range(1, 5);
            _lastAttack = randomAttack;
            _currentAttackIndex = randomAttack + 5;

            SignToNextAttack(randomAttack - 1, false);
            yield return new WaitForSeconds(1f);

            switch (randomAttack)
            {
                case 1:
                    _inScriptAttack = StartCoroutine(ArcShooting(_arcProjectileCount));
                    yield return new WaitUntil(() => _finishedCoroutine);
                    break;
                case 2:
                    TsukuyomiAllRoundFire?.Invoke();
                    yield return new WaitForSeconds(_allRoundFire.GetAttackTime());
                    break;
                case 3:
                    _inScriptAttack = StartCoroutine(RandomShooting());
                    yield return new WaitUntil(() => _finishedCoroutine);
                    break;
                case 4:
                    _inScriptAttack = StartCoroutine(AxisShooting());
                    yield return new WaitUntil(() => _finishedCoroutine);
                    break;
            }
            yield return new WaitForSeconds(_attackDelay);
        }
    }
    #region Susanoo attacks
    private IEnumerator ArcShooting(int currentProjectileCount)
    {
        FlashSphere();
        _finishedCoroutine = false;
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
                GameObject currentProjectile = _poolManager.GetObject(_randomShootProjectile);
                currentProjectile.transform.position = transform.position;
                float currentAngle = startAngle + (i + angleCof) * currentAttackAngle;
                if (currentAngle > 360f) currentAngle -= 360f;
                else if (currentAngle < 0f) currentAngle += 360f;
                currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, currentAngle);
                yield return new WaitForSeconds(_arcDelay);
            }
            yield return new WaitForSeconds(_arcWaveDelay);
        }
        _finishedCoroutine = true;
    }
    private IEnumerator RandomShooting()
    {
        _finishedCoroutine = false;
        for (int i = 0; i < _randomProjectileCount; i++)
        {
            _audioManager.PlaySFX("Susanoo Shot");

            GameObject currentProjectile = _poolManager.GetObject(_randomShootProjectile);
            currentProjectile.transform.position = transform.position;
            currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f));
            yield return new WaitForSeconds(_randomDelay);
        }
        _finishedCoroutine = true;
    }
    private IEnumerator AxisShooting()
    {
        _finishedCoroutine = false;
        for (int i = 0; i < _axisCount; i++) 
        {
            GameObject currentProjectile = _poolManager.GetObject(_axisProjectile);
            _audioManager.PlaySFX("Susanoo Bubble");
            yield return new WaitForSeconds(_axisDelay);
        }
        yield return new WaitForSeconds(6f);
        _finishedCoroutine = true;
    }
    private IEnumerator SpawnEnemiesAttack()
    {
        Vector2 randomPlace = _spawnPoints;
        bool changeX = false;
        FlashSphere();
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(_summonDelay);
            _audioManager.PlaySFX("Susanoo Minions summon");
            GameObject spawnedEnemy = _poolManager.GetObject(_enemy);
            spawnedEnemy.transform.position = randomPlace;
            _poolManager.GetObject(_enemyAppearParticle).transform.position = randomPlace;
            if (changeX)
            {
                randomPlace.x *= -1;
            }
            else
            {
                randomPlace.y *= -1;
            }
            changeX = !changeX;
        }
    }
    private void SpawnTwoSmiles()
    {
        Vector2 spawnPos = _spawnSmilePos;
        if(UnityEngine.Random.Range(0, 2) == 0)
        {
            spawnPos.x *= -1;
        }
        GameObject smile = Instantiate(_smiles, spawnPos, Quaternion.identity);
        _smileList.Add(smile);
        spawnPos.y *= -1;
        smile.transform.DOMove(spawnPos, _smileDuration);
        Destroy(smile, _smileDuration);
    }
    #endregion

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
        yield return new WaitForSeconds(1f);
        Vector3 originalPos = transform.localPosition;
        float shakeTimer = 0f;

        while (shakeTimer < 2f)
        {
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * 0.1f;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * 0.1f;

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            shakeTimer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
        StartCoroutine(WinLight());
        this.enabled = false;
    }
    private IEnumerator WinLight()
    {
        for(float i = 1f; i < 100f; i += 50f * Time.deltaTime)
        {
            _sun.intensity = i;
            yield return null;
        }
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
        switch (_currentAttackIndex)
        {
            case 1:
                foreach(GameObject smile in _smileList)
                {
                    if (smile != null)
                    {
                        Destroy(smile);
                    }
                }
                _smileList.Clear();
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
                break;
            case 6:
                if (_inScriptAttack != null)
                {
                    StopCoroutine(_inScriptAttack);
                    _inScriptAttack = null;
                }
                break;
            case 7:
                _allRoundFire.StopAttack();
                break;
            case 8:
                if (_inScriptAttack != null)
                {
                    StopCoroutine(_inScriptAttack);
                    _inScriptAttack = null;
                }
                break;
            case 9:
                if (_inScriptAttack != null)
                {
                    StopCoroutine(_inScriptAttack);
                    _inScriptAttack = null;
                }
                break;
        }
        _finishedCoroutine = true;
    }
    #endregion

    private void CheckPhaseTransition(float healthPart)
    {
        if (_healthManager.GetHealth() / _healthManager._maxHealth <= 0.5f && _phase < 2)
        {
            _phase = 2;
            _animator.SetInteger(_phaseAnim, 2);

            _teleport.repeatTimes += 1;
            _teleport.teleportDuration -= 0.1f;
            _teleport.projectileCount += 4;
            _teleport.attackInterval -= 0.1f;

            _allRoundFire.rows += 4;

            _lunarDiskAttack.repeatTimes++;

            _axisCount += 2;
            _arcDelay -= 0.05f;
            _arcWaveDelay -= 0.15f;

            _randomDelay -= 0.1f;
            _randomProjectileCount += 2;

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
        if (_attackCycle == null)
        {
            _attackCycle = StartCoroutine(StartAttacks());
        }
    }
    private void FlashSphere()
    {
        _audioManager.PlaySFX("Boss Casting");
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
