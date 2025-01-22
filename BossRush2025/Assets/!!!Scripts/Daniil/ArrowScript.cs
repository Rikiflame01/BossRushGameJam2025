using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;
    [SerializeField] private ParticleSystem _splash;

    private string _obstacleLayerName = "Obstacles";

    private PoolManager _poolManager;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private bool _canMove = true;

    void Start()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _poolManager = FindAnyObjectByType<PoolManager>();
    }
    void Update()
    {
        if (_canMove)
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
            SplashDisappear();
        }
        else if (other.CompareTag("Ritual"))
        {
            SplashDisappear();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer(_obstacleLayerName))
        {
            SplashDisappear();
        }
    }
    private void SplashDisappear()
    {
        _splash.Play();
        _spriteRenderer.enabled = false;
        StartCoroutine(Disappear());
    }
    private IEnumerator Disappear()
    {
        _canMove = false;
        _collider.enabled = false;
        yield return new WaitForSeconds(3f);
        _collider.enabled = true;
        _spriteRenderer.enabled = true;
        _canMove = true;
        _poolManager.ReturnObject(gameObject, "Arrow");
    }
}
