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
    [SerializeField] private List<float> _particleSpeed;
    [SerializeField] private List<float> _particleCount;
    [SerializeField] private List<float> _particleMaxSize;
    [SerializeField] private List<float> _particleMinLifeTime;
    [SerializeField] private List<float> _particleMaxLifeTime;
    private Light2D _light2D;
    private ParticleSystem _particleSystem;
    void Awake()
    {
        _poolManager = FindAnyObjectByType<PoolManager>();
        _light2D = GetComponentInChildren<Light2D>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
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

        float currentRadius = _lightRadius[circleNumber];
        float startTime = Time.time;
        while(Time.time - startTime < _lightAccelerateDuration)
        {
            _light2D.pointLightOuterRadius = currentRadius * ((Time.time - startTime) / _lightAccelerateDuration);
            yield return null;
        }
        _light2D.pointLightOuterRadius = currentRadius;
        yield return new WaitForSeconds(0.8f);
        startTime = Time.time;
        while (Time.time - startTime < _lightAccelerateDuration)
        {
            _light2D.pointLightOuterRadius = currentRadius * (1 - ((Time.time - startTime) / _lightAccelerateDuration));
            yield return null;
        }
        _light2D.pointLightOuterRadius = 0f;
        yield return new WaitForSeconds(1f);
        _poolManager.ReturnObject(gameObject, _name);
    }
}
