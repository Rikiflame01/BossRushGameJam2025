using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Teleport : MonoBehaviour
{
    public string pointName;
    public string projectileName;
    public Transform arenaBounds;
    private Transform player;
    public int projectileCount = 12;
    public int repeatTimes = 3;
    public float projectileSpeed = 10f;
    public float teleportDuration = 0.5f;
    public float attackInterval = 1f;
    public float minDistanceFromPlayer = 2f;
    public float maxDistanceFromPlayer = 10f;
    public float minDistanceBetweenPoints = 2f;

    [SerializeField] private string _teleportParticles;
    public bool _finishedAttack { get; private set; } = true;

    private Coroutine _teleportCoroutine, _translateCoroutine;

    private List<GameObject> instantiatedPoints = new List<GameObject>();
    private List<Vector3> teleportPoints = new List<Vector3>();
    private Vector3 originalPosition;
    private void Start()
    {
        player = FindAnyObjectByType<Movement>().transform;
        originalPosition = transform.position;

        TsukuyomiBoss.TsukuyomiTeleport += StartTeleportSequence;
    }

    private void OnDestroy()
    {
        TsukuyomiBoss.TsukuyomiTeleport -= StartTeleportSequence;
    }
    private void StartTeleportSequence()
    {
        _teleportCoroutine = StartCoroutine(TeleportAndAttackRoutine());
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
    }
    private IEnumerator TeleportAndAttackRoutine()
    {
        _finishedAttack = false;
        teleportPoints.Clear();

        FireProjectiles(transform.position);
        yield return new WaitForSeconds(attackInterval);

        for (int i = 0; i < repeatTimes; i++)
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

            GameObject point = PoolManager._instance.GetObject(pointName);
            point.transform.position = randomPosition;
            instantiatedPoints.Add(point);
        }

        foreach (Vector3 point in teleportPoints)
        {
            yield return StartCoroutine(SmoothTeleport(point));
            FireProjectiles(point);
            yield return new WaitForSeconds(attackInterval);
        }

        yield return _translateCoroutine = StartCoroutine(SmoothTeleport(originalPosition));

        foreach (GameObject point in instantiatedPoints)
        {
            PoolManager._instance.ReturnObject(point, pointName);
        }
        instantiatedPoints.Clear();
        teleportPoints.Clear();
        _finishedAttack = true;
    }
    private IEnumerator SmoothTeleport(Vector3 targetPosition)
    {
        float index = 1f;
        for(float i = 1f; i >= 0f; i -= 0.2f)
        {
            transform.DOScaleX(i * index, 0.15f * Mathf.Abs(i));
            index *= -1f;
            yield return new WaitForSeconds(0.15f * Mathf.Abs(i));
        }
        PoolManager._instance.GetObject(_teleportParticles).transform.position = transform.position;
        yield return new WaitForSeconds(teleportDuration);
        PoolManager._instance.GetObject(_teleportParticles).transform.position = targetPosition;
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.15f);


        transform.position = targetPosition;
    }
    /*private IEnumerator SmoothTeleport(Vector3 targetPosition)
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
    }*/

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

            GameObject projectile = PoolManager._instance.GetObject(projectileName);
            projectile.transform.position = transform.position;
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = projectileMoveDirection * projectileSpeed;
                rb.gravityScale = 0;
            }
        }
    }
    public void StopTeleport()
    {
        if(_teleportCoroutine != null)
        {
            StopCoroutine(_teleportCoroutine);
            _teleportCoroutine = null;
        }
        if (_translateCoroutine != null)
        {
            StopCoroutine(_translateCoroutine);
            _translateCoroutine = null;
        }
        foreach (GameObject point in instantiatedPoints)
        {
            PoolManager._instance.ReturnObject(point, pointName);
        }
        instantiatedPoints.Clear();
        transform.localScale = Vector3.one;
        _finishedAttack = true;
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
