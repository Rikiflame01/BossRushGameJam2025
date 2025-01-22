using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextEffect : MonoBehaviour
{
    [SerializeField] private GameObject _textPrefab;
    private HealthManager _healthManager;
    
    void Start()
    {
        _healthManager = GetComponent<HealthManager>();
        _healthManager._onHit += SpawnText;
    }

    void SpawnText(float damage)
    {
        GameObject text = Instantiate(_textPrefab, transform.position, Quaternion.identity);
        text.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"-{Math.Round((decimal)damage, 2)} Hp";
        Destroy(text, 1f);
    }

    void OnDestroy()
    {
        _healthManager._onHit -= SpawnText;
    }
}
