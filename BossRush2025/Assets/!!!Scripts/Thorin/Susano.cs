/*
 * AUthor:Thorin
 * Description: sample boss that inherits the entity state abstract class
 * intention implement custom logic here.
 */
using UnityEngine;
using System.Collections;
public class Susano : EntityState
{
    [Header("Shooting")]
    [SerializeField] private float _chargeTime;
    [SerializeField] private float _angleBetweenProjectiles;
    [SerializeField] private float _projectileCount;
    [SerializeField] private string _shootProjectile;

    private Transform _player;
    private PoolManager _poolManager;
    void Awake()
    {
        _player = FindAnyObjectByType<Movement>().transform;
        _poolManager = FindAnyObjectByType<PoolManager>();
    }
    protected override void HandleIdle() { Debug.Log("Enemy B Idle Behavior"); }
    protected override void HandleWalking() { Debug.Log("Enemy B Walking Behavior"); }
    protected override void HandleRunning() { Debug.Log("Enemy B Running Behavior"); }
    private IEnumerator Shooting()
    {
        yield return new WaitForSeconds(_chargeTime);
        Vector2 direction = _player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        int helpInt = 1;
        for (int i = 0; i < _projectileCount; i++) 
        {
            angle += i * helpInt * _angleBetweenProjectiles;
            GameObject currentProjectile = _poolManager.GetObject(_shootProjectile);
            currentProjectile.transform.eulerAngles = new Vector3(0f, 0f, angle + i * helpInt * _angleBetweenProjectiles);
            currentProjectile.transform.position = transform.position;
            helpInt *= -1;
        }
    }
}
