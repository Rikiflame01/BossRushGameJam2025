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
