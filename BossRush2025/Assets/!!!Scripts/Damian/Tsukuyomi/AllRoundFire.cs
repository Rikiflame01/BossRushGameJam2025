using UnityEngine;
using System.Collections;

public class AllRoundFire : MonoBehaviour
{
    public string projectileName = "Projectile";
    public int rows = 3;
    public int projectilesPerRow = 6;
    public float angleOffset = 10f;
    public float projectileSpeed = 2f;
    public float rowDelay = 0.2f;

    public bool _finishedAttack { get; private set; } = true;

    private Coroutine _fireCoroutine;

    private void Start()
    {
        TsukuyomiBoss.TsukuyomiAllRoundFire += FireAllRound;
        AmaterasuBoss.TsukuyomiAllRoundFire += FireAllRound;
    }

    private void OnDestroy()
    {
        TsukuyomiBoss.TsukuyomiAllRoundFire -= FireAllRound;
        AmaterasuBoss.TsukuyomiAllRoundFire -= FireAllRound;
    }


    public void FireAllRound()
    {
        _fireCoroutine = StartCoroutine(FireAllRoundWithDelay());
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
        _finishedAttack = true;
    }
    public float GetAttackTime()
    {
        return rowDelay * rows;
    }
    public void StopAttack()
    {
        if(_fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
            _fireCoroutine = null;
        }
        _finishedAttack = true;
    }
}
