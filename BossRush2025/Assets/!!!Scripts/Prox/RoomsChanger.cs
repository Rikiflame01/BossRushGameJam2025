using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomsChanger : MonoBehaviour
{
    [SerializeField] private int _nextRoomIndex = 2;
    [SerializeField] private Fade _fade;
    private bool _canChangeRoom = false;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        GameManager._instance.BossDefeat += ()=> {
            _canChangeRoom = true;
            _spriteRenderer.enabled = true;
        };
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.TryGetComponent<Movement>(out Movement moevement))
        {
            if (_canChangeRoom)
            {
                ChangeRoom();
            }
        }
    }

    public void ChangeRoom()
    {
        StartCoroutine(ChangeRoomWithDelay());
    }

    private IEnumerator ChangeRoomWithDelay()
    {
        _fade.FadeIn();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_nextRoomIndex);
    }
}
