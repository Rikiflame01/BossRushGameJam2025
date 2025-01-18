using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField] private float _waveSpeed = 10f;
    [SerializeField] private float _waveDamage = 2f;
    private int _horizontalMoveDirection = 0;
    private WaveAttack _waveAttack;

    void Start()
    {

    }

    void Update()
    {
        transform.position += transform.right * _horizontalMoveDirection * _waveSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Movement>(out Movement playerMovement))
        {
            if (playerMovement.TryGetComponent<HealthManager>(out HealthManager healthManager))
            {
                healthManager.TakeDamage(_waveDamage);
            }

            if (playerMovement.TryGetComponent<Knockback>(out Knockback knockBack))
            {
                Vector2 knockbackDirection = Vector2.zero;
                if (transform.position.y > playerMovement.transform.position.y)
                {
                    knockbackDirection.y = 1;
                }
                else
                {
                    knockbackDirection.y = -1;
                }

                knockBack.PlayKnockBack(new Vector2(playerMovement.transform.position.x, playerMovement.transform.position.y) + knockbackDirection, 10f, 0.8f);
            }

            CameraShake._instance.Shake(1.0f, 0.25f);
        }

        if (other.transform.tag == "Barrier")
        {
            Destroy(gameObject);
        }
    }

    public void SetMoveDirection(int moveDirection)
    {
        _horizontalMoveDirection = moveDirection;
    }

    public void SetWaveAttack(WaveAttack waveAttack)
    {
        _waveAttack = waveAttack;
    }
}
