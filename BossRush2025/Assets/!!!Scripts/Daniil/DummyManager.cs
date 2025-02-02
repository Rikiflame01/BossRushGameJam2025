using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class DummyManager : MonoBehaviour
{
    [SerializeField] private RectTransform _dashText;
    [SerializeField] private RectTransform _swordStrikeText;
    [SerializeField] private RectTransform _bowText;
    [SerializeField] private RectTransform _beatText;
    [SerializeField] private RectTransform _ritualText;
    [SerializeField] private RectTransform _goText;
    [SerializeField] private HealthManager _dummyHealth;
    private bool _isRitual = false, _alreadyDie;
    void Start()
    {
        StartCoroutine(Tutotial());
        AudioManager._instance.PlayBGM("Main menu");
        _dummyHealth._onHit += CheckPhaseTransition;
        _dummyHealth._onDie += Die;
        _dummyHealth.SetImmortal();
    }
    private void CheckPhaseTransition(float healthPart)
    {
        if (_dummyHealth.GetHealth() / _dummyHealth._maxHealth <= 0.5f && !_isRitual) 
        {
            _beatText.DOScale(0f, 0.15f);
            _ritualText.DOScale(1f, 0.15f);
            StartCoroutine(RitualTextDisappear());
            GameManager._instance.RitualBegin();
            _isRitual = true;
        }
    }
    private void Die()
    {
        if (!_alreadyDie)
        {
            GameManager._instance.DefeatedBoss();
            _alreadyDie = true;
            _goText.DOScale(1f, 0.15f);
        }
    }
    private void OnDestroy()
    {
        _dummyHealth._onHit -= CheckPhaseTransition;
        _dummyHealth._onDie -= Die;
    }
    private IEnumerator Tutotial()
    {
        yield return new WaitForSeconds(4f);
        _dashText.DOScale(1f, 0.15f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        _dashText.DOScale(0f, 0.15f);
        _swordStrikeText.DOScale(1f, 0.15f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.X));
        _swordStrikeText.DOScale(0f, 0.15f);

        _bowText.DOScale(1f, 0.15f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.C));
        _bowText.DOScale(0f, 0.15f);
        _dummyHealth.ReturnImmortal();

        _beatText.DOScale(1f, 0.15f);
    }
    private IEnumerator RitualTextDisappear()
    {
        yield return new WaitForSeconds(1.15f);
        yield return new WaitUntil(() => Input.GetKey(KeyCode.X));

        _ritualText.DOScale(0f, 0.15f);
    }
}
