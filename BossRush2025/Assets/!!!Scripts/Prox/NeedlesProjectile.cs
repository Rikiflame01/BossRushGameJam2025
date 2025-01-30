using System.Collections;
using UnityEngine;

public class NeedlesProjectile : MonoBehaviour
{
    [SerializeField] private float _damage = 5f;
    [SerializeField] private float _rotateSpeed = 5f;
    [SerializeField] private float _flySpeed = 8f;

    [SerializeField] private string _destroyParticles = "CrystalSplash";
 
    private GameObject _target;
    private Vector3 _flyDirection;
    private bool _attack = false;
    private bool _lookAtTarget = false;
    private bool _spin = true;

    void Start()
    {
        StartCoroutine(RotateDelay());
    }

    void Update()
    {
        
        if (_attack)
        {
            transform.position += _flyDirection * _flySpeed * Time.deltaTime;
        }

        if (_spin)
        {
            transform.eulerAngles += new Vector3(0, 0, _rotateSpeed * Time.deltaTime);
        }
        
        if (_lookAtTarget)
        {
            LookAtTarget();
        }
    }

    void LookAtTarget()
    {
        Vector3 look = transform.InverseTransformPoint(_target.transform.position);
        float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg;

        transform.Rotate(0, 0, angle);
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    private IEnumerator RotateDelay()
    {
        yield return new WaitForSeconds(1f);
        transform.eulerAngles = Vector3.zero;
        _spin = false; 
        _lookAtTarget = true;
        yield return new WaitForSeconds(1f); 
        _lookAtTarget = false;
        _attack = true;

        _flyDirection = -(transform.position - _target.transform.position).normalized;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_attack)
        {
            if (other.transform.TryGetComponent<Movement>(out Movement playerMovement))
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
                PoolManager._instance.GetObject(_destroyParticles).transform.position = transform.position;
                Destroy(gameObject);
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                PoolManager._instance.GetObject(_destroyParticles).transform.position = transform.position;
                Destroy(gameObject);
            }
        }
    }
}
