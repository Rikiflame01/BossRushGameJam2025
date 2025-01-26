using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private InputActionReference _pauseKey;
    private bool _paused = false;

    void Start()
    {
        _pauseKey.action.performed += (InputAction.CallbackContext context)=> PauseTrigger();
    }

    public void PauseTrigger()
    {
        _paused = !_paused;
        _pauseMenu.SetActive(_paused);
        Time.timeScale = _paused ? 0f : 1f;
    }

    void OnDestroy()
    {
        _pauseKey.action.performed -= (InputAction.CallbackContext context)=> PauseTrigger();
    }
}
