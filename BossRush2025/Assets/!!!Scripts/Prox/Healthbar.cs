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

    void UpdateHealthBar(float healthPart)
    {
        _healthBar.fillAmount = _healthManager.GetHealth()  / _healthManager._maxHealth;
    }
}
