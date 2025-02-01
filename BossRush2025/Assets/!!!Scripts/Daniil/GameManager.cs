using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _ritualCircle;
    [SerializeField] private string _burnParticles;
    [SerializeField] private string _sakuraWave;

    [SerializeField] private HealthManager _bossHealth;
    [SerializeField] private List<float> _burnDamage;
    [SerializeField] private float _burnDelay;
    [SerializeField] private int _numberOfBurns;

    [Header("Start BossFight")]
    [SerializeField] private Transform _backgroundMask;
    [SerializeField] private GameObject _boss;
    [SerializeField] private GameObject _bossStatue;
    [SerializeField] private GameObject _bossNameText;
    [SerializeField] private ParticleSystem _sakuraLeaves;

    private PoolManager _poolManager;
    public float RitualCircleRadius;
    public Transform RitualCenter;

    public event Action StartFight;
    public event Action RitualStart, RitualFinished;
    public event Action PlayerDie, BossDefeat;

    public static GameManager _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        _poolManager = FindAnyObjectByType<PoolManager>();
        FindAnyObjectByType<Movement>().GetComponent<HealthManager>()._onDie += GameOver;
    }
    public void StartGame()
    {
        StartCoroutine(StartBossFight());
    }
    private IEnumerator StartBossFight()
    {
        CameraFollow._instance.ToCenter(6f);
        yield return new WaitForSeconds(0.5f);
        _boss.SetActive(true);
        _backgroundMask.DOScale(15f, 2f);
        _sakuraLeaves.Play();
        AudioManager._instance.PlaySFX("StartBossFight");
        yield return new WaitForSeconds(4f);
        _bossNameText.SetActive(true);
        _bossStatue.SetActive(false);
        StartFight?.Invoke();
    }
    public void RitualBegin()
    {
        RitualStart?.Invoke();
        _ritualCircle.SetActive(true);
        _ritualCircle.GetComponent<Collider2D>().enabled = true;
        GameObject _currentParticles = _poolManager.GetObject(_sakuraWave);
        _currentParticles.transform.position = Vector3.zero;
        AudioManager._instance.PlaySFX("Boss sitting");
    }

    public void RitualEnd(int circleNumber)
    {
        RitualFinished?.Invoke();
        CameraFollow._instance.ToCenter();
        StartCoroutine(BurnBoss(circleNumber-1));
        AudioManager._instance.PlaySFX("Seal and burn");
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
            if (_bossHealth.enabled)
            {
                _bossHealth.TakeDamage(singleDamage);
                yield return new WaitForSeconds(_burnDelay);
            }
        }
    }

    private void GameOver()
    {
        PlayerDie?.Invoke();
    }

    public void DefeatedBoss()
    {
        BossDefeat?.Invoke();
        StartCoroutine(WaitToMask());
    }
    private IEnumerator WaitToMask()
    {
        yield return new WaitForSeconds(3f);
        _backgroundMask.DOScale(0f, 2f);
    }
}
