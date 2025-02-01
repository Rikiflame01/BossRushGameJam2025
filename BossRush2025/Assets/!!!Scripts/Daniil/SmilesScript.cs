using UnityEngine;
using System.Collections;

public class SmilesScript : MonoBehaviour
{
    public string projectileName = "Projectile";
    public int projectilesPerRow = 6;
    public float angleOffset = 10f;
    public float projectileSpeed = 2f;
    public float rowDelay = 0.2f;

    [SerializeField] private float _speedRotation;

    void Start()
    {
        StartCoroutine(FireAllRoundWithDelay());
    }
    void Update()
    {
        transform.Rotate(Vector3.forward, _speedRotation * Time.deltaTime);
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

            yield return new WaitForSeconds(rowDelay);
        }
    }
}
