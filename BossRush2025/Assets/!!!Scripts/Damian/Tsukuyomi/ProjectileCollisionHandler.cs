using UnityEngine;

public class ProjectileCollisionHandler : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private float _damage = 5f;
    [SerializeField] private string _destroyParticles;
    private string _obstacleLayerName = "Obstacles";
    private Collider2D projectileCollider;

    private void Start()
    {
        projectileCollider = GetComponent<Collider2D>();

        if (projectileCollider == null)
        {
            Debug.LogError("No Collider2D found on the projectile. Please add one.");
            return;
        }

        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            if (col.CompareTag("Player") || col.gameObject.layer == LayerMask.NameToLayer(_obstacleLayerName))
                continue;
            Physics2D.IgnoreCollision(projectileCollider, col);
        }
    }
    protected void ReturnInPool()
    {
        GameObject _currentParticles = PoolManager._instance.GetObject(_destroyParticles);
        _currentParticles.transform.position = transform.position;
        _currentParticles.GetComponent<ParticleSystem>().Play();
        PoolManager._instance.ReturnObject(gameObject, _name);
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
