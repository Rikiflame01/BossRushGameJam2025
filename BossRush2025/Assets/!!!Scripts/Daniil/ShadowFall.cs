using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ShadowFall : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private Vector2 _startScale;
    [SerializeField] private Vector2 _endScale;
    [SerializeField] private Color _startColor;
    [SerializeField] private Color _endColor;
    [SerializeField] private float _duration;
    private SpriteRenderer _spriteRenderer;
    private PoolManager _poolManager;
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _poolManager = FindAnyObjectByType<PoolManager>();
    }
    void OnEnable()
    {
        transform.localScale = _startScale;
        _spriteRenderer.color = _startColor;
        transform.DOScale(_endScale, _duration);
        _spriteRenderer.DOColor(_endColor, _duration);
        StartCoroutine(ReturnInPool());
    }
    private IEnumerator ReturnInPool()
    {
        yield return new WaitForSeconds(_duration);
        _poolManager.ReturnObject(gameObject, _name);
    }
}
