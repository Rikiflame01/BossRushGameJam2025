using UnityEngine;
using System.Collections;

public class BossRoar : MonoBehaviour
{
    [SerializeField] private string _whiteParticles;
    [SerializeField] private string _roarParticles;

    [SerializeField] private int _countWhiteParticles;
    [SerializeField] private float _forceWhiteParticles;
    [SerializeField] private float _delayWhiteParticles;

    [SerializeField] private int _countRoarParticles;
    [SerializeField] private float _delayRoarParticles;

    private PoolManager _poolManager;

    private void Start()
    {
        _poolManager = FindAnyObjectByType<PoolManager>();

        StartCoroutine(SpawnRoarParticles());
        StartCoroutine(SpawnWhiteParticles());
        //Destroy(gameObject, 5f);
    }
    private IEnumerator SpawnWhiteParticles()
    {
        for (int i = 0; i < _countWhiteParticles; i++)
        {
            GameObject currentProjectile = _poolManager.GetObject(_whiteParticles);
            currentProjectile.transform.position = transform.position;
            Vector2 direction = Random.insideUnitCircle.normalized;
            currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            currentProjectile.GetComponent<Rigidbody2D>().AddForce(currentProjectile.transform.right * _forceWhiteParticles);
            yield return new WaitForSeconds(_delayWhiteParticles);
        }
    }
    private IEnumerator SpawnRoarParticles()
    {
        for (int i = 0; i < _countRoarParticles; i++)
        {
            GameObject currentProjectile = _poolManager.GetObject(_roarParticles);
            currentProjectile.transform.position = transform.position;
            yield return new WaitForSeconds(_delayRoarParticles);
        }
    }
}
