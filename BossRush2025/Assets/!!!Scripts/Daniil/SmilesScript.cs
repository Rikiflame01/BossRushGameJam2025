using UnityEngine;
using System.Collections;

public class SmilesScript : MonoBehaviour
{
    public string projectileName = "Projectile";
    public int projectilesPerRow = 6;
    public float angleOffset = 10f;
    public float projectileSpeed = 2f;
    public float rowDelay = 0.2f;

    private Transform _sprite;

    [SerializeField] private float _speedRotation;

    void Start()
    {
        StartCoroutine(FireAllRoundWithDelay());
        _sprite = transform.GetChild(0);
    }
    public void Initialize(int newProjectilesPerRow, float newRowDelay, float newAngleOffset)
    {
        projectilesPerRow = newProjectilesPerRow;
        rowDelay = newRowDelay;
        angleOffset = newAngleOffset; if(Random.Range(0, 2) == 0) { angleOffset *= -1; }
    }
    void Update()
    {
        _sprite.Rotate(Vector3.forward, _speedRotation * Time.deltaTime);
    }
    private IEnumerator FireAllRoundWithDelay()
    {
        yield return new WaitForSeconds(0.8f);
        for(int row = 0; row < 100; row++)
        {
            float baseAngle = row * angleOffset;

            for (int i = 0; i < projectilesPerRow; i++)
            {
                float angle = baseAngle + (360f / projectilesPerRow) * i;
                Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);

                GameObject projectile = PoolManager._instance.GetObject(projectileName);
                projectile.transform.position = transform.position;
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.linearVelocity = direction * projectileSpeed;
                }
            }
            AudioManager._instance.PlaySFX("Amaterasu Fireball");
            yield return new WaitForSeconds(rowDelay);
        }
    }
}
