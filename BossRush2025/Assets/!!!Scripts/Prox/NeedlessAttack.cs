using System.Collections;
using UnityEngine;

public class NeedlessAttack : MonoBehaviour
{
    [SerializeField] private GameObject _needlesProjectilePrefab;
    [SerializeField] private float _circleRadius = 3f;
    private int _countOfProjectiles = 8;
    private GameObject _player;
    private bool _stopAttack = false;

    void Start()
    {
        TsukuyomiBoss._tsukuyomiNeedlesAttack += ()=> StartCoroutine(SpawnProjectiles());
        
        _player = GameObject.FindFirstObjectByType<Movement>().gameObject;
    }

    private IEnumerator SpawnProjectiles()
    {
        for (int i = 0; i < _countOfProjectiles; i++)
        {
            if (_stopAttack)
            {
                _stopAttack = false;
                yield break;
            }

            yield return new WaitForSeconds(2.5f);

            if (_stopAttack)
            {
                _stopAttack = false;
                yield break;
            }

            Vector3 spawnProjectilePos = Random.insideUnitCircle * _circleRadius;
            GameObject spawnedProjectile = Instantiate(_needlesProjectilePrefab, spawnProjectilePos, Quaternion.identity);
            spawnedProjectile.GetComponent<NeedlesProjectile>().SetTarget(_player);
        }
    }

    public void StopAttack()
    {
        _stopAttack = true;
    }
}
