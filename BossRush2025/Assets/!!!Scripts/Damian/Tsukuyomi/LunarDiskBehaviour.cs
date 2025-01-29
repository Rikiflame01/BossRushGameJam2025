using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class LunarDiskBehaviour : MonoBehaviour
{
    [SerializeField] private float _damage = 5f;

    private float moveSpeed;
    private float rotateSpeed;
    private float lifeTime;

    private GameObject projectilePrefab;
    private float projectileSpeed;
    private float projectileLifeTime;

    private Rigidbody2D rb;
    private int obstacleLayer;

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
        GameObject projectilePrefab, float projectileSpeed, float projectileLifetime)
    {
        this.moveSpeed = diskSpeed;
        this.rotateSpeed = spinSpeed;
        this.lifeTime = diskLifetime;

        this.projectilePrefab = projectilePrefab;
        this.projectileSpeed = projectileSpeed;
        this.projectileLifeTime = projectileLifetime;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        obstacleLayer = LayerMask.NameToLayer("Obstacles");
    }

    private void Start()
    {
        TsukuyomiBoss._tsukuyomiLunarDiskAttack += HandleLunarDisk;

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = randomDir * moveSpeed;

        StartCoroutine(DiskLifetimeRoutine());
    }

    private void OnDisable()
    {
        TsukuyomiBoss._tsukuyomiLunarDiskAttack -= HandleLunarDisk;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider, true);
            return;
        }

        if (collision.gameObject.layer == obstacleLayer)
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 oppositeDirection = -normal.normalized;
            float bounceForce = moveSpeed;

            rb.AddForce(oppositeDirection * bounceForce, ForceMode2D.Impulse);
        }

        if (collision.gameObject.CompareTag("Player")){
            if (collision.gameObject.TryGetComponent<HealthManager>(out HealthManager healthManager))
            {
                healthManager.TakeDamage(_damage);
            }
        }
    }
}
