using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class SwordStrikeAttack : MonoBehaviour
{
    [SerializeField] private float _damage = 5f;
    [SerializeField] private float _followSpeed = 5f;
    [SerializeField] private float _followTime = 1f;
    [SerializeField] private float _areaDamageSize = 3f;
    private GameObject _player;
    private Rigidbody2D _rb;
    private bool _stopAttack = false;
    public bool _finishedAttack { get; private set;}

    void Start()
    {
        _player = GameObject.FindFirstObjectByType<Movement>().gameObject;
        _rb = GetComponent<Rigidbody2D>();

        TsukuyomiBoss._tsukuyomiSwordStrikeAttack += ()=> StartCoroutine(StartSwordStrikeAttack());
    }

    void Update()
    {
        
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

            yield return new WaitForSeconds(1f);

            if (_stopAttack)
            {
                _stopAttack = false;
                _finishedAttack = true;
                yield break;
            }

            _rb.linearVelocity = -(transform.position - _player.transform.position).normalized * _followSpeed;
            yield return new WaitForSeconds(_followTime);
            _rb.linearVelocity = Vector2.zero;
            AreaDamage();
        }

        _finishedAttack = true;
    }

    void AreaDamage()
    {
        List<RaycastHit2D> collidingObjects = Physics2D.CircleCastAll(transform.position, _areaDamageSize, Vector2.zero).ToList();
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
                    knockBack.PlayKnockBack(transform.position, 5f, 0.25f);
                }

                CameraShake._instance.Shake(0.5f, 0.5f);
                break;
            }
        }
    }

    public void StopAttack()
    {
        _stopAttack = true;
    }
}
