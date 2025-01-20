using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _ritualCircle;
    [SerializeField] private string _burnParticles;

    [SerializeField] private HealthManager _bossHealth;
    [SerializeField] private List<float> _burnDamage;
    [SerializeField] private float _burnDelay;
    [SerializeField] private int _numberOfBurns;

    private PoolManager _poolManager;
    public float RitualCircleRadius;
    public Transform RitualCenter;
    public event Action RitualStart, RitualFinished;
    void Start()
    {
        _poolManager = FindAnyObjectByType<PoolManager>();
    }
    public void RitualBegin()
    {
        RitualStart?.Invoke();
        _ritualCircle.SetActive(true);
    }
    public void RitualEnd(int circleNumber)
    {
        RitualFinished?.Invoke();
        StartCoroutine(BurnBoss(circleNumber-1));
        GameObject _currentParticles = _poolManager.GetObject(_burnParticles);
        if (_currentParticles.TryGetComponent<RitualParticlesScript>(out RitualParticlesScript particleScript))
        {
            particleScript.SetCircleNumber(circleNumber);
        }

        _ritualCircle.SetActive(false);
    }
    private IEnumerator BurnBoss(int circleNumber)
    {
        float singleDamage = _burnDamage[circleNumber] / _numberOfBurns;
        for (int i = 0; i < _numberOfBurns; i++)
        {
            _bossHealth.TakeDamage(singleDamage);
            yield return new WaitForSeconds(_burnDelay);
        }
    }
}
