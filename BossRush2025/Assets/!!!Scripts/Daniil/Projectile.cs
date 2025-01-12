using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected string _name;
    [SerializeField] private float _damage;
    protected PoolManager _poolManager;
    void Start()
    {
        _poolManager = FindAnyObjectByType<PoolManager>();
        Initialize();
    }
    protected abstract void Initialize();
    protected void ReturnInPool()
    {
        _poolManager.ReturnObject(gameObject, _name);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager healthManager = other.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.TakeDamage(_damage);
            }
            if (other.TryGetComponent<Knockback>(out Knockback knockBack))
            {
                knockBack.PlayKnockBack(transform.position);
            }
            CameraShake._instance.Shake();
        }
    }
}
