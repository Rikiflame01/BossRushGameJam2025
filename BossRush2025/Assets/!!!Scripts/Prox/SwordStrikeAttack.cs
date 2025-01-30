using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class SwordStrikeAttack : MonoBehaviour
{
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackDelay = 0.35f;
    [SerializeField] private float _damage = 5f;
    [SerializeField] private float _followSpeed = 5f;
    [SerializeField] private float _followTime = 1f;
    [SerializeField] private float _areaDamageSize = 3f;
    private GameObject _player;
    private bool _stopAttack = false;

    private bool _needMove = false;
    private Vector2 _direction;

    public bool _finishedAttack { get; private set; } = true;

    void Start()
    {
        _player = GameObject.FindFirstObjectByType<Movement>().gameObject;

        TsukuyomiBoss._tsukuyomiSwordStrikeAttack += ()=> StartCoroutine(StartSwordStrikeAttack());
    }
    void FixedUpdate()
    {
        if (_needMove)
        {
            transform.Translate(_direction * _followSpeed * Time.fixedDeltaTime);
        }
    }
    public IEnumerator StartSwordStrikeAttack()
    {
        _finishedAttack = false;

        int repeatTimes = 3;
        for (int i = 0; i < repeatTimes; i++)
        {
            if (_stopAttack)
            {
                _stopAttack = false;
                _finishedAttack = true;
                yield break;
            }
            _needMove = true;
            _direction = -(transform.position - _player.transform.position).normalized;
            yield return new WaitForSeconds(_followTime);
            _needMove = false;
            AreaDamage();
            yield return new WaitForSeconds(_attackDelay);
            if (_stopAttack)
            {
                _stopAttack = false;
                _finishedAttack = true;
                yield break;
            }
        }

        _finishedAttack = true;
    }

    void AreaDamage()
    {
        List<RaycastHit2D> collidingObjects = Physics2D.CircleCastAll(_attackPoint.position, _areaDamageSize, Vector2.zero).ToList();
        foreach (RaycastHit2D hit in collidingObjects)
        {
            if (hit.collider.transform.TryGetComponent<Movement>(out Movement playerMovement))
            {
                if (playerMovement.TryGetComponent<HealthManager>(out HealthManager healthManager))
                {
                    healthManager.TakeDamage(_damage);
                }

                if (playerMovement.TryGetComponent<Knockback>(out Knockback knockBack))
                {
                    knockBack.PlayKnockBack(_attackPoint.position, 5f, 0.25f);
                }

                CameraShake._instance.Shake(0.5f, 0.5f);
                break;
            }
        }
    }

    public void StopAttack()
    {
        _stopAttack = true;
        _needMove = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackPoint.position, _areaDamageSize);
    }
}
