using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _damage;
    private PoolManager _poolManager;
    void Start()
    {
        _poolManager = FindAnyObjectByType<PoolManager>();
    }
    void Update()
    {
        transform.Translate(Vector2.right * _speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<HealthManager>(out HealthManager healthManager))
            {
                healthManager.TakeDamage(_damage);
            }
            if (other.TryGetComponent<Knockback>(out Knockback knockBack))
            {
                knockBack.PlayKnockBack(transform.position);
            }
        }
    }
    private void Disappear()
    {
        _poolManager.ReturnObject(gameObject, "Arrow");
    }
}
