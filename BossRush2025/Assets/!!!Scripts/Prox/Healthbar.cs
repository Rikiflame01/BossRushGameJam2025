using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private HealthManager _healthManager;

    void Start()
    {
        _healthManager._onHit += UpdateHealthBar;
    }

    void UpdateHealthBar(float health)
    {
        _healthBar.fillAmount = health / _healthManager._maxHealth;
    }
}
