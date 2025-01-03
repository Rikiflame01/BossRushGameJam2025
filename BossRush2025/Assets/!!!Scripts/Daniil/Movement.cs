using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D _playerRb;
    [SerializeField] private float _speed;
    private bool _disable;
    [SerializeField] private InputActionReference _moveInput;
    [SerializeField] private float _hitStunTime;
    void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
    }
    private Vector2 Direction() => _moveInput.action.ReadValue<Vector2>();
    void FixedUpdate()
    {
        if (!_disable)
        {
            _playerRb.linearVelocity = Direction() * _speed;
        }
    }
    public void TakeHit()
    {
        if (!_disable)
            StartCoroutine(Stun(_hitStunTime));
    }
    private IEnumerator Stun(float time)
    {
        _disable = true;
        yield return new WaitForSeconds(_hitStunTime);
        _disable = false;
    }
}
