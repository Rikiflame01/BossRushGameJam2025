using UnityEngine;

public class CrescentAttack : MonoBehaviour
{
    [SerializeField] private GameObject _crescentMoonPrefab;
    private GameObject _player;

    void Awake()
    {
        TsukuyomiBoss._tsukuyomiCrescentAttack += StartCrescentAttack;
        _player = GameObject.FindFirstObjectByType<Movement>().gameObject;
    }

    void Start()
    {
    }

    public void StartCrescentAttack()
    {
        GameObject spawnedPrefab = Instantiate(_crescentMoonPrefab, _player.transform.position + new Vector3(Random.Range(-3, 3), 0, 0), Quaternion.identity);
    }

    void OnDestroy()
    {
        TsukuyomiBoss._tsukuyomiCrescentAttack -= StartCrescentAttack;  
    }
}
