using UnityEngine;
using System.Collections;

public class AllRoundFire : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int rows = 3;
    public int projectilesPerRow = 6;
    public float angleOffset = 10f;
    public float projectileSpeed = 2f;
    public float projectileLifetime = 5f;
    public float rowDelay = 0.2f;

    public bool _finishedAttack { get; private set; }

    private void Start()
    {
        TsukuyomiBoss.TsukuyomiAllRoundFire += FireAllRound;
    }

    private void OnDestroy()
    {
        TsukuyomiBoss.TsukuyomiAllRoundFire -= FireAllRound;
    }


    public void FireAllRound()
    {
        StartCoroutine(FireAllRoundWithDelay());
    }

    private IEnumerator FireAllRoundWithDelay()
    {
        _finishedAttack = false;
        for (int row = 0; row < rows; row++)
        {
            float baseAngle = row * angleOffset;

            for (int i = 0; i < projectilesPerRow; i++)
            {
                float angle = baseAngle + (360f / projectilesPerRow) * i;
                Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);

                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.linearVelocity = direction * projectileSpeed;
                }

                Destroy(projectile, projectileLifetime);
            }

            yield return new WaitForSeconds(rowDelay);
        }
        _finishedAttack = true;
    }

}
