using System.Collections;
using UnityEngine;

public class CrescentMoon : MonoBehaviour
{
    [SerializeField] private float _damage = 5f;
    private Animator _animator;
    private bool _canDamage = false;

    void Start()
    {
        _animator = GetComponent<Animator>();    
        _animator.SetTrigger("Attack");
        StartCoroutine(DamageDelay());
    }

    private IEnumerator DamageDelay()
    {
        yield return new WaitForSeconds(2.0f);
        _canDamage = true;
        yield return new WaitForSeconds(2.0f);
        _canDamage = false;
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    { 
        if (!_canDamage)
            return;

       Damage(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!_canDamage)
            return;

        Damage(other);
    }

    void Damage(Collider2D other)
    {
        if (other.TryGetComponent<Movement>(out Movement playerMovement))
        {
            if (playerMovement.TryGetComponent<HealthManager>(out HealthManager healthManager))
            {
                healthManager.TakeDamage(_damage);
                CameraShake._instance.Shake();
            }
        }

        _canDamage = false;
    }
}
