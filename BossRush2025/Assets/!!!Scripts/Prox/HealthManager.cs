using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private float _health = 10f;
    private float _maxHealth;
    private bool _isAlive = true;
    private bool _receivceDamage = true;
    public event Action _onHit;
    public event Action _onDie;

    void Start()
    {
        _maxHealth = _health;
    }

    public void TakeDamage(float damage)
    {
        if (!_isAlive || !_receivceDamage)
            return;

        _health = Math.Clamp(_health - damage, 0, _maxHealth);
        _onHit?.Invoke();
        if (_health == 0)
        {
            _onDie?.Invoke();
            Destroy(gameObject);
        }
    }

    public void AddHealth(float health)
    {
        if (!_isAlive)
            return;

        _health = Math.Clamp(_health + health, 0, _maxHealth);
    }

    public void SetHealth(float health)
    {
        if (!_isAlive)
            return;

        _health = Math.Clamp(health, 0, _maxHealth);
    }

    public void EnableReceivingDamage()
    {
        _receivceDamage = true;
    }

    public void DisableReceivingDamage()
    {
        _receivceDamage = false;
    }
}
