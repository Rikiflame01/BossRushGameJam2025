using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;

public class RitualParticlesScript : MonoBehaviour
{
    private PoolManager _poolManager;
    [SerializeField] private string _name;
    [SerializeField] private List<float> _lightRadius;
    [SerializeField] private float _lightAccelerateDuration = 0.25f;
    [SerializeField] private float _lightDuration = 0.8f;
    [SerializeField] private List<float> _particleSpeed;
    [SerializeField] private List<float> _particleCount;
    [SerializeField] private List<float> _particleMaxSize;
    [SerializeField] private List<float> _particleMinLifeTime;
    [SerializeField] private List<float> _particleMaxLifeTime;

    private ParticleSystem _particleSystem;
    private Flash _flash;

    void Awake()
    {
        _poolManager = FindAnyObjectByType<PoolManager>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _flash = GetComponentInChildren<Flash>();
    }
    public void SetCircleNumber(int circleNumber)
    {
        StartCoroutine(StartParticles(circleNumber));
    }
    private IEnumerator StartParticles(int circleNumber)
    {
        circleNumber--;
        var emission = _particleSystem.emission;
        emission.rateOverTime = _particleCount[circleNumber];
        var main = _particleSystem.main;
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, _particleMaxSize[circleNumber]);
        main.startLifetime = new ParticleSystem.MinMaxCurve(_particleMinLifeTime[circleNumber], _particleMaxLifeTime[circleNumber]);
        main.startSpeed = _particleSpeed[circleNumber];
        _particleSystem.Play();

        _flash.LightOn(_lightRadius[circleNumber], _lightAccelerateDuration, _lightDuration);
        yield return new WaitForSeconds(2f);
        _poolManager.ReturnObject(gameObject, _name);
    }
}
