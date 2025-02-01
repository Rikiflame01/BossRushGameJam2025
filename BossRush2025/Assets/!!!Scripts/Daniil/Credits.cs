using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Credits : MonoBehaviour
{
    [SerializeField] private float _delay = 5f;
    [SerializeField] private GameObject _credits;
    [SerializeField] private UnityEvent _onGameEnd;

    void Start()
    {
        GameManager._instance.BossDefeat += ()=>{ StartCoroutine(ActivateCreditsWithDelay()); _onGameEnd?.Invoke(); };
    }

    IEnumerator ActivateCreditsWithDelay()
    {
        yield return new WaitForSeconds(_delay);
        _credits.SetActive(true);
    }
}
