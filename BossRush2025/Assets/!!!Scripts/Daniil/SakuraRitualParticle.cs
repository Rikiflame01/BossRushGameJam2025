using UnityEngine;

public class SakuraRitualParticle : MonoBehaviour
{
    [SerializeField] private Color _startColor;
    [SerializeField] private Color _endColor;
    [SerializeField] private float _startSize;
    [SerializeField] private float _endSize;
    private SpriteRenderer _spriteRenderer;
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        ChangeParameteres();
    }
    void OnEnable()
    {
        ChangeParameteres();
    }
    void ChangeParameteres()
    {
        _spriteRenderer.color = Color.Lerp(_startColor, _endColor, Random.Range(0f, 1f));
        transform.localScale = Vector3.one * Random.Range(_startSize, _endSize);
    }
}
