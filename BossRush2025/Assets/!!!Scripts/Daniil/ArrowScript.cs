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
        if (other.transform.TryGetComponent<Enemy>(out Enemy enemy))
        {
            if (enemy.TryGetComponent<HealthManager>(out HealthManager healthManager))
            {
                healthManager.TakeDamage(_damage);
            }

            if (enemy.TryGetComponent<Knockback>(out Knockback knockBack))
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
