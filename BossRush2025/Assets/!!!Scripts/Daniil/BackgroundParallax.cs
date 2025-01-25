using UnityEngine;
using System.Collections.Generic;

public class BackgroundParallax : MonoBehaviour
{
    [SerializeField] private List<float> _relativeSpeed;
    private List<Transform> _backgroundLayers = new List<Transform>();

    private Transform _cameraTransform;
    private Vector2 _currentVector;
    private List<float> _originalDistancesX = new List<float>();
    void Start()
    {
        _currentVector = transform.GetChild(0).position;
        _cameraTransform = Camera.main.transform;
        foreach(Transform layer in transform)
        {
            _backgroundLayers.Add(layer);
            _originalDistancesX.Add(layer.position.x - _cameraTransform.position.x);
        }
    }
    void Update()
    {
        for(int i = 0; i < _backgroundLayers.Count; i++)
        {
            _currentVector.x = _cameraTransform.position.x * _relativeSpeed[i] + _originalDistancesX[i];
            transform.GetChild(i).position = _currentVector;
        }
    }
}

