using UnityEngine;

public class StartBossFightTrigger : MonoBehaviour
{
    private GameManager _gameManager;
    void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _gameManager.StartGame();
            Destroy(gameObject);
        }
    }
}
