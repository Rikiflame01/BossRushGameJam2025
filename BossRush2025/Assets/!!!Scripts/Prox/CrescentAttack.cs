using UnityEngine;

public class CrescentAttack : MonoBehaviour
{
    [SerializeField] private GameObject _crescentMoonPrefab;
    private GameManager _gameManager;
    private GameObject _player;

    void Awake()
    {
        TsukuyomiBoss._tsukuyomiCrescentAttack += StartCrescentAttack;
        _player = GameObject.FindFirstObjectByType<Movement>().gameObject;

        _gameManager = FindAnyObjectByType<GameManager>();
    }
    public void StartCrescentAttack()
    {
        GameObject spawnedPrefab = Instantiate(_crescentMoonPrefab, _gameManager.RitualCenter.position, Quaternion.identity);
        spawnedPrefab.transform.localScale = Vector3.one * 2 * _gameManager.RitualCircleRadius;
        spawnedPrefab.transform.Rotate(Vector3.forward, 180f);
    }

    void OnDestroy()
    {
        TsukuyomiBoss._tsukuyomiCrescentAttack -= StartCrescentAttack;  
    }
}
