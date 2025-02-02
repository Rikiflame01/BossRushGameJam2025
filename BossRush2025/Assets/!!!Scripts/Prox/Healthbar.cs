using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private HealthManager _healthManager;

    [Header("Take Damage Animation")]
    [SerializeField] private Image _healthBarEffect;
    [SerializeField] private float _takeDamageAnimSpeed = 3f;
    [SerializeField] private float _startAnimDelay = 0.5f;
    private bool _startAnim = false;

    void Start()
    {
        _healthManager._onHit += (float hitDamage)=> { if (gameObject.activeSelf) { UpdateHealthBar(hitDamage); StartCoroutine(StartAnimDelay()); } };
        _healthManager._onAddHealth += (float heal)=> { if (gameObject.activeSelf) { UpdateHealthBar(heal); StartCoroutine(StartAnimDelay()); } };
    }

    void Update()
    {
        if (_startAnim)
        {
            float fillAmount = _healthManager.GetHealth() / _healthManager._maxHealth;
            if (fillAmount != _healthBarEffect.fillAmount)
            {
                _healthBarEffect.fillAmount = Math.Clamp(_healthBarEffect.fillAmount + ((fillAmount < _healthBarEffect.fillAmount ? -1 : 1) * _takeDamageAnimSpeed * Time.deltaTime), fillAmount, 1);
            }
            else
                _startAnim = false;
        }
    }

    void UpdateHealthBar(float healthPart)
    {
        _healthBar.fillAmount = _healthManager.GetHealth()  / _healthManager._maxHealth;
    }

    private IEnumerator StartAnimDelay()
    {
        yield return new WaitForSeconds(_startAnimDelay);
        _startAnim = true;
    }
}
