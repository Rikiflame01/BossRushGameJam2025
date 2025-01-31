using System.Collections;
using UnityEngine;

public class WaveAttack : MonoBehaviour
{
    [SerializeField] private GameObject _wavePrefab;
    [SerializeField] private int _countOfWavesInAttack = 0;
    [SerializeField] private float _waveSpawnDelay = 3f;
    [SerializeField] private Transform _upSpawnWavePosition, _downSpawnWavePosition;
    private int _countOfWaves = 0;
    private bool _finishedAttack = false;

    private Coroutine _waveCoroutine;
    
    void Start()
    {
        Susano._sussanoWaveAttack += StartWaveAttack;
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.RitualStart += StopWaveAttack;
        gameManager.BossDefeat += StopWaveAttack;
    }
    
    public void StartWaveAttack()
    {
        _countOfWaves = 0;
        _finishedAttack = false;
        SpawnWave();
    }

    public void SpawnWave()
    {
        if (_countOfWaves >= _countOfWavesInAttack)
        {
            _finishedAttack = true;
            return;
        }
        AudioManager._instance.PlaySFX("Susanoo Wave attack");

        int randomWavePos = Random.Range(0, 2);
        Wave wave = Instantiate(_wavePrefab, randomWavePos == 0 ? _upSpawnWavePosition.position : _downSpawnWavePosition.position, Quaternion.identity).GetComponent<Wave>();
        wave.SetMoveDirection(-1);
        _countOfWaves++;

        _waveCoroutine = StartCoroutine(SpawnWavesDelay());
    }

    private IEnumerator SpawnWavesDelay()
    {
        yield return new WaitForSeconds(_waveSpawnDelay);
        SpawnWave();
    }

    public float GetAttackTime()
    {
        return _countOfWavesInAttack * _waveSpawnDelay;
    }

    void OnDestroy()
    {
        Susano._sussanoWaveAttack -= StartWaveAttack;
    }
    private void StopWaveAttack()
    {
        if(_waveCoroutine != null)
        {
            StopCoroutine(_waveCoroutine);
            _waveCoroutine = null;
        }
    }
}
