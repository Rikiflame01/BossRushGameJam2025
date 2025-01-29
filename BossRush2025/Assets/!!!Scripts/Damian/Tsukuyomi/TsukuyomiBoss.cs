using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsukuyomiBoss : EntityState
{
    public static event Action TsukuyomiTeleport;
    //TsukuyomiTeleport?.Invoke(); to trigger the attack
    public static event Action TsukuyomiAllRoundFire;
    //TsukuyomiAllRoundFire?.Invoke(); to trigger the attack
    public static event Action _tsukuyomiCrescentAttack;
    public static event Action _tsukuyomiSwordStrikeAttack;
    [SerializeField] float _attackDelay;

    private bool _finishedCoroutine = true;
    private int _lastAttack = -1;

    protected override void HandleIdle() { Debug.Log("Enemy B Idle Behavior"); }
    protected override void HandleWalking()
    {
        if (!(_navMeshAgent.pathPending || _moveWaiting || _navMeshAgent.remainingDistance != 0)) StartCoroutine(MoveWaiting());
        if (_finishedCoroutine)
        {
            FlipToPlayer();
        }
    }
    protected override void HandleRunning() { Debug.Log("Enemy B Running Behavior"); }
    
    protected override void Initialize()
    {
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
                    break;

                case 2:
                    break;

                case 3:
                    break;

                case 4:
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
            int randomAttack = UnityEngine.Random.Range(1, 4);

            SignToNextAttack(randomAttack - 1, false);
            yield return new WaitForSeconds(1f);

            switch (randomAttack)
            {
                case 1:
                    break;

                case 2:
                    break;

                case 3:
                    break;
            }

            if (!_finishedCoroutine)
                yield return new WaitUntil(() => _finishedCoroutine);
            yield return new WaitForSeconds(_attackDelay);
        }
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

    }
    void OnDestroy()
    {
        
    }
}
