using UnityEngine;

public class CollisionEnemyDamage : MonoBehaviour
{    
    [SerializeField] private float _damage = 5f;
    [SerializeField] private float _lifetime = 3f;
    [SerializeField] private string _destroyParticles;
    private Rigidbody2D rb;
    private Vector2 savedVelocity;
    private bool isPaused = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        TsukuyomiBoss._tsukuyomiLunarDiskAttack += OnBossCommand;

    }

    private void OnDisable()
    {
        TsukuyomiBoss._tsukuyomiLunarDiskAttack -= OnBossCommand;
        PoolManager._instance.GetObject(_destroyParticles).transform.position = transform.position;
    }

    private void OnBossCommand(string command)
    {
        switch (command)
        {
            case "Pause":
                PauseProjectile();
                break;
            case "Resume":
                ResumeProjectile();
                break;
        }
    }

    private void PauseProjectile()
    {
        if (isPaused) return;
        isPaused = true;

        savedVelocity = rb.linearVelocity;
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
    }

    private void ResumeProjectile()
    {
        if (!isPaused) return;
        isPaused = false;

        rb.isKinematic = false;
        rb.linearVelocity = savedVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")){
            if (collision.gameObject.TryGetComponent<HealthManager>(out HealthManager healthManager))
            {
                healthManager.TakeDamage(_damage);
            }
        }
    }
}
