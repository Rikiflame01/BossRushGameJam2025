using System.Collections;
using UnityEngine;

public class NeedlessAttack : MonoBehaviour
{
    [SerializeField] private GameObject _needlesProjectilePrefab;
    [SerializeField] private float _circleRadius = 2f;
    [SerializeField] private float _delay = 1f;
    [SerializeField] private int _countOfProjectiles = 8;
    private GameObject _player;
    private bool _stopAttack = false;

    public bool _finishedAttack { get; private set; } = true;

    void Start()
    {
        TsukuyomiBoss._tsukuyomiNeedlesAttack += ()=> StartCoroutine(SpawnProjectiles());
        
        _player = GameObject.FindFirstObjectByType<Movement>().gameObject;
    }

    private IEnumerator SpawnProjectiles()
    {
        _finishedAttack = false;
        for (int i = 0; i < _countOfProjectiles; i++)
        {
            if (_stopAttack)
            {
                _stopAttack = false;
                yield break;
            }

            yield return new WaitForSeconds(_delay);

            if (_stopAttack)
            {
                _stopAttack = false;
                yield break;
            }

            Vector2 spawnProjectilePos = GetRandomPointOnCircle();
            GameObject spawnedProjectile = Instantiate(_needlesProjectilePrefab, spawnProjectilePos, Quaternion.identity);
            spawnedProjectile.GetComponent<NeedlesProjectile>().SetTarget(_player);
        }
        _finishedAttack = false;
    }
    Vector2 GetRandomPointOnCircle()
    {
        Vector2 center = GameManager._instance.RitualCenter.position;
        float radius = GameManager._instance.RitualCircleRadius + _circleRadius;
        float angle = Random.Range(0f, Mathf.PI * 2);
        float x = center.x + Mathf.Cos(angle) * radius;
        float y = center.y + Mathf.Sin(angle) * radius;
        return new Vector2(x, y);
    }
    public void StopAttack()
    {
        _stopAttack = true;
    }
}
