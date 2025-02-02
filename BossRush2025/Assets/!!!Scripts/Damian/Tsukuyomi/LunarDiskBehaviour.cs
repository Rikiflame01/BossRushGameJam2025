using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class LunarDiskBehaviour : MonoBehaviour
{
    [SerializeField] private float _damage = 5f;
    [SerializeField] private string _destroyParticles;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float lifeTime;

    private GameObject projectilePrefab;
    private float projectileSpeed;
    private float projectileLifeTime;

    private Rigidbody2D rb;
    private int obstacleLayer;

    private bool isVertical;
    private bool isPaused = false;
    private Vector2 savedVelocity;
    private float savedRotateSpeed;
    private float elapsedLifetime = 0f;

    public void PauseDisk()
    {
        if (isPaused) return;

        isPaused = true;
        savedVelocity = rb.linearVelocity;
        savedRotateSpeed = rotateSpeed;

        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        rotateSpeed = 0f;
    }

    public void ResumeDisk()
    {
        if (!isPaused) return;

        isPaused = false;

        rb.isKinematic = false;
        rb.linearVelocity = savedVelocity;
        rotateSpeed = savedRotateSpeed;
    }

    public void Initialize(
        float diskSpeed, float spinSpeed, float diskLifetime,
        GameObject projectilePrefab, float projectileSpeed, float projectileLifetime, bool isVertical)
    {
        this.moveSpeed = diskSpeed;
        this.rotateSpeed = spinSpeed;
        this.lifeTime = diskLifetime;

        this.projectilePrefab = projectilePrefab;
        this.projectileSpeed = projectileSpeed;
        this.projectileLifeTime = projectileLifetime;
        this.isVertical = isVertical;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        obstacleLayer = LayerMask.NameToLayer("Obstacles");
    }

    private void Start()
    {
        TsukuyomiBoss._tsukuyomiLunarDiskAttack += HandleLunarDisk;
        AmaterasuBoss._tsukuyomiLunarDiskAttack += HandleLunarDisk;

        if(!isVertical)
        {
            Vector2 randomDir = Random.Range(0, 2) == 0 ? Vector2.right : Vector2.left;
            rb.linearVelocity = randomDir * moveSpeed;
        }
        else
        {
            Vector2 randomDir = Random.Range(0, 2) == 0 ? Vector2.up : Vector2.down;
            rb.linearVelocity = randomDir * moveSpeed;
        }
        StartCoroutine(DiskLifetimeRoutine());
    }

    private void OnDisable()
    {
        TsukuyomiBoss._tsukuyomiLunarDiskAttack -= HandleLunarDisk;
        AmaterasuBoss._tsukuyomiLunarDiskAttack -= HandleLunarDisk;
        //GameObject _currentParticles = PoolManager._instance.GetObject(_destroyParticles);
        //_currentParticles.transform.position = transform.position;
    }

    private void HandleLunarDisk(string command)
    {
        switch (command)
        {
            case "Pause":
                PauseDisk();
                break;
            case "Resume":
                ResumeDisk();
                break;
        }
    }

    private void Update()
    {
        if (!isPaused)
        {
            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        }
    }

    private IEnumerator DiskLifetimeRoutine()
    {
        elapsedLifetime = 0f;

        while (elapsedLifetime < lifeTime)
        {
            while (isPaused)
            {
                yield return null;
            }

            elapsedLifetime += Time.deltaTime;
            yield return null;
        }
        AudioManager._instance.PlaySFX("Tsukuyomi moon spliting");

        SpawnProjectile(Vector2.up);
        SpawnProjectile(Vector2.left);
        SpawnProjectile(Vector2.down);
        SpawnProjectile(Vector2.right);
        Debug.Log("Additional Projectile is spawned");

        Destroy(gameObject);
    }

    private void SpawnProjectile(Vector2 direction)
    {
        if (projectilePrefab == null) return;

        GameObject projObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projRb = projObj.GetComponent<Rigidbody2D>();

        if (projRb != null)
        {
            projRb.linearVelocity = direction * projectileSpeed;
        }

        Destroy(projObj, projectileLifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == obstacleLayer)
        {
            if (isVertical)
            {
                if ((rb.linearVelocity.y > 0 && transform.position.y > 0) || (rb.linearVelocity.y < 0 && transform.position.y < 0))
                    rb.linearVelocity = -rb.linearVelocity;
            }
            else
            {
                if ((rb.linearVelocity.x > 0 && transform.position.x > 0) || (rb.linearVelocity.x < 0 && transform.position.x < 0))
                    rb.linearVelocity = -rb.linearVelocity;
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.TryGetComponent<HealthManager>(out HealthManager healthManager))
            {
                healthManager.TakeDamage(_damage);
            }
            if (collision.TryGetComponent<Knockback>(out Knockback knockBack))
            {
                knockBack.PlayKnockBack(transform.position);
            }
            CameraShake._instance.Shake();
        }
    }
}
