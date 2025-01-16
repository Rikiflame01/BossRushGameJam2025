using System;
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
    public float teleportDuration = 0.5f;
    public float attackInterval = 1f;
    public float minDistanceFromPlayer = 2f;
    public float maxDistanceFromPlayer = 5f;
    public float minDistanceBetweenPoints = 2f;
    public float projectileLifetime = 5f;

    private List<Vector3> teleportPoints = new List<Vector3>();
    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;

        HealthManager.TsukuyomiTeleport += StartTeleportSequence;
    }

    private void OnDestroy()
    {
        HealthManager.TsukuyomiTeleport -= StartTeleportSequence;
    }

    private void StartTeleportSequence()
    {
        StartCoroutine(TeleportAndAttackRoutine());
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
    }
private IEnumerator TeleportAndAttackRoutine()
{
    teleportPoints.Clear();
    List<GameObject> instantiatedPoints = new List<GameObject>();

    FireProjectiles(transform.position);
    yield return new WaitForSeconds(attackInterval);

    for (int i = 0; i < 3; i++)
    {
        Vector3 randomPosition;
        int maxAttempts = 10;
        int attempts = 0;

        do
        {
            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
            float randomDistance = UnityEngine.Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
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

        teleportPoints.Add(randomPosition);

        GameObject point = Instantiate(pointPrefab, randomPosition, Quaternion.identity);
        instantiatedPoints.Add(point);
    }

    foreach (Vector3 point in teleportPoints)
    {
        yield return StartCoroutine(SmoothTeleport(point));
        FireProjectiles(point);
        yield return new WaitForSeconds(attackInterval);
    }

    yield return StartCoroutine(SmoothTeleport(originalPosition));

    foreach (GameObject point in instantiatedPoints)
    {
        Destroy(point);
    }

    teleportPoints.Clear();
}

    private IEnumerator SmoothTeleport(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < teleportDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / teleportDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
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

        foreach (Vector3 point in teleportPoints)
        {
            if (Vector3.Distance(position, point) < minDistanceBetweenPoints)
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
        float randomAngle = UnityEngine.Random.Range(0f, 360f);
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

        Destroy(projectile, projectileLifetime);
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
