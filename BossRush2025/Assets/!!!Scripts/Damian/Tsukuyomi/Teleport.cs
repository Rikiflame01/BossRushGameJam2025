using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject projectilePrefab;
    public Transform arenaBounds;
    public Transform player;
    public int projectileCount = 12;
    public float projectileSpeed = 5f;
    public float teleportDelay = 1f;
    public float attackInterval = 1f;
    public float pointDuration = 10f;
    public float minDistanceFromPlayer = 2f;
    public float maxDistanceFromPlayer = 5f;
    public float minDistanceBetweenPoints = 2f;

    private List<Transform> points = new List<Transform>();
    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
        StartCoroutine(TeleportAndAttackRoutine());
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
    }
    private IEnumerator TeleportAndAttackRoutine()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 randomPosition;
            int maxAttempts = 10;
            int attempts = 0;

            do
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                float randomDistance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
                randomPosition = (Vector3)(randomDirection * randomDistance) + player.position;
                randomPosition.z = 0;

                attempts++;
                if (attempts > maxAttempts)
                {
                    Debug.LogWarning("Failed to place a point at a valid distance.");
                    break;
                }
            }
            while (!IsValidPoint(randomPosition));

            GameObject point = Instantiate(pointPrefab, randomPosition, Quaternion.identity);
            points.Add(point.transform);
        }

        FireProjectiles(originalPosition);

        foreach (Transform point in points)
        {
            yield return new WaitForSeconds(teleportDelay);

            transform.position = point.position;

            transform.rotation = Quaternion.identity;

            foreach (Transform attackPoint in points)
            {
                FireProjectiles(attackPoint.position);
            }

            FireProjectiles(originalPosition);

            yield return new WaitForSeconds(attackInterval);
        }

        foreach (Transform point in points)
        {
            Destroy(point.gameObject);
        }
        points.Clear();

        transform.position = originalPosition;

        transform.rotation = Quaternion.identity;
    }

    private bool IsValidPoint(Vector3 position)
    {
        if (arenaBounds != null)
        {
            Vector3 boundsMin = arenaBounds.position - arenaBounds.localScale / 2;
            Vector3 boundsMax = arenaBounds.position + arenaBounds.localScale / 2;

            if (position.x < boundsMin.x || position.x > boundsMax.x ||
                position.y < boundsMin.y || position.y > boundsMax.y)
            {
                return false;
            }
        }

        foreach (Transform point in points)
        {
            if (Vector3.Distance(position, point.position) < minDistanceBetweenPoints)
            {
                return false;
            }
        }

        return true;
    }

private void FireProjectiles(Vector3 position)
{
    for (int i = 0; i < projectileCount; i++)
    {
        float randomAngle = Random.Range(0f, 360f);
        float projectileDirX = Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        float projectileDirY = Mathf.Sin(randomAngle * Mathf.Deg2Rad);

        Vector3 projectileMoveDirection = new Vector3(projectileDirX, projectileDirY, 0).normalized;

        GameObject projectile = Instantiate(projectilePrefab, position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = projectileMoveDirection * projectileSpeed;
            rb.gravityScale = 0;
        }

        Destroy(projectile, 5f);
    }
}



    private void OnDrawGizmos()
    {
        if (arenaBounds != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(arenaBounds.position, arenaBounds.localScale);
        }
    }
}
