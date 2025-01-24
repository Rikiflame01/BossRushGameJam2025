using UnityEngine.Rendering.Universal;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light2D))]

public class Flash : MonoBehaviour
{
    public Light2D _light2D;

    void Awake()
    {
        _light2D = GetComponent<Light2D>();
    }
    public void LightOn(float _originalRadius, float _lightAccelerateDuration, float _lightDuration)
    {
        StartCoroutine(LightCoroutine(_originalRadius, _lightAccelerateDuration, _lightDuration));
    }
    private IEnumerator LightCoroutine(float _originalRadius, float _lightAccelerateDuration, float _lightDuration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < _lightAccelerateDuration)
        {
            _light2D.pointLightOuterRadius = _originalRadius * ((Time.time - startTime) / _lightAccelerateDuration);
            yield return null;
        }
        _light2D.pointLightOuterRadius = _originalRadius;
        yield return new WaitForSeconds(_lightDuration);
        startTime = Time.time;
        while (Time.time - startTime < _lightAccelerateDuration)
        {
            _light2D.pointLightOuterRadius = _originalRadius * (1 - ((Time.time - startTime) / _lightAccelerateDuration));
            yield return null;
        }
        _light2D.pointLightOuterRadius = 0f;
    }
}
