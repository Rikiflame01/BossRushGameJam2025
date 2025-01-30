using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected string _name;
    [SerializeField] private float _damage;
    [SerializeField] private string _destroyParticles;

    private string _obstacleLayerName = "Obstacles";

    protected PoolManager _poolManager;
    void Start()
    {
        _poolManager = FindAnyObjectByType<PoolManager>();
        Initialize();
    }
    protected abstract void Initialize();
    protected void ReturnInPool()
    {
        if (gameObject.activeSelf)
        {
            GameObject _currentParticles = _poolManager.GetObject(_destroyParticles);
            _currentParticles.transform.position = transform.position;
            _poolManager.ReturnObject(gameObject, _name);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<HealthManager>(out HealthManager healthManager))
            {
                healthManager.TakeDamage(_damage);
            }
            if (other.TryGetComponent<Knockback>(out Knockback knockBack))
            {
                knockBack.PlayKnockBack(transform.position);
            }
            CameraShake._instance.Shake();
            ReturnInPool();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer(_obstacleLayerName))
        {
            ReturnInPool();
        }
    }
}
