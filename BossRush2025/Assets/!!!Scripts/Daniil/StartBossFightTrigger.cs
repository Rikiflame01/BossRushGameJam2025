using UnityEngine;

public class StartBossFightTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager._instance.StartGame();
            Destroy(gameObject);
        }
    }
}
