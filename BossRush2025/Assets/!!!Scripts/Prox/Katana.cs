using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Katana : MonoBehaviour
{
    [Header("Katana Settings")]
    [SerializeField] private float _katanaDamage = 5f;
    [SerializeField] private float _attackDelay = 0.5f;
    [SerializeField] private float _attackRadius = 3f;

    [Space]
    [SerializeField] private Transform _katanaAttackPos;
    [SerializeField] private InputActionReference _katanaAttackKey;
    [SerializeField] private Animator _animator;
    private bool _canAttack = true;

    private string _katanaSound = "Katana ";
    private AudioManager _audioManager;

    void Start()
    {
        _audioManager = FindAnyObjectByType<AudioManager>();

        _katanaAttackKey.action.performed += Attack;
    }

    void Update()
    {

    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!_canAttack)
            return;

        _animator.SetTrigger("KatanaAttack");
        List<RaycastHit2D> collidingObjects = Physics2D.CircleCastAll(_katanaAttackPos.position, _attackRadius, Vector2.zero).ToList();
        foreach (RaycastHit2D hit in collidingObjects)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                GameObject enemy = hit.collider.gameObject;
                if (enemy.TryGetComponent<HealthManager>(out HealthManager healthManager))
                {
                    healthManager.TakeDamage(_katanaDamage);
                }

                if (enemy.TryGetComponent<Knockback>(out Knockback knockBack))
                {
                    knockBack.PlayKnockBack(_katanaAttackPos.position);
                }

                CameraShake._instance.Shake();
            }
        }
        _canAttack = false;
        _audioManager.PlaySFX(_katanaSound + Random.Range(1, 4));

        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(_attackDelay);
        _canAttack = true;
    }
    void OnDisable()
    {
        _katanaAttackKey.action.performed -= Attack;
    }
    void OnEnable()
    {
        _katanaAttackKey.action.performed += Attack;
    }
    void OnDestroy()
    {
        _katanaAttackKey.action.performed -= Attack;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_katanaAttackPos.position, _attackRadius);
    }
}
