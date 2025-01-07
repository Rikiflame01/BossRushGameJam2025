/*
 * AUthor:Thorin
 * Description: sample boss that inherits the entity state abstract class
 * intention implement custom logic here.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Susano : EntityState
{
    [SerializeField] private List<Component> _componentsToDisableOnDisappear;

    [Header("Shooting")]
    [SerializeField] private float _chargeTime;
    [SerializeField] private float _angleBetweenProjectiles;
    [SerializeField] private float _projectileCount;
    [SerializeField] private string _shootProjectile;

    private Transform _player;
    private PoolManager _poolManager;

    [Header("Melee Attack")]
    [SerializeField] private float _circualSwordStrikeAttackRadius = 5f;
    [SerializeField] private float _circualSwordStrikeAttackDamage = 6f;

    [Space]
    [SerializeField] private float _jumpAttackAppearNearRadius = 6f;

    private SpriteRenderer _spriteRenderer;
    private bool _finishedCoroutine = false;

    protected override void HandleIdle() { Debug.Log("Enemy B Idle Behavior"); }
    protected override void HandleWalking() { Debug.Log("Enemy B Walking Behavior"); }
    protected override void HandleRunning() { Debug.Log("Enemy B Running Behavior"); }

    protected override void Initialize()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _player = FindAnyObjectByType<Movement>().transform;
        _poolManager = FindAnyObjectByType<PoolManager>();

        StartCoroutine(StartAttacks());    
    }

    IEnumerator StartAttacks()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(StartMeleeAttack());
        yield return new WaitUntil(()=> _finishedCoroutine == true);
    }

    IEnumerator StartMeleeAttack()
    {
        Debug.Log("Start Melee Attack");
        _finishedCoroutine = false;

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
                Vector2 randomPositionInCircle = Random.insideUnitCircle * _jumpAttackAppearNearRadius;
                transform.position = playerPosition + randomPositionInCircle;
                Appear();
            }
            else if (randomAttack == 1)
            {
                lastCircualSwordStrike = true;

                List<RaycastHit2D> collidingObjects = Physics2D.CircleCastAll(transform.position, _circualSwordStrikeAttackRadius, Vector2.zero).ToList();
                foreach (RaycastHit2D hit in collidingObjects)
                {
                    if (hit.collider.transform.TryGetComponent<Movement>(out Movement playerMovement))
                    {
                        if (playerMovement.TryGetComponent<HealthManager>(out HealthManager healthManager))
                        {
                            healthManager.TakeDamage(_circualSwordStrikeAttackDamage);
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

            yield return new WaitForSeconds(1);
        }

        _finishedCoroutine = true;
    }

    private IEnumerator Shooting()
    {
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
}
