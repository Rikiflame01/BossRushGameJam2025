using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    private int _appearAnim = Animator.StringToHash("Appear");
    private Animator _animatorUI;

    void Start()
    {
        GameManager._instance.PlayerDie += ActiveUI;
    }
    private void ActiveUI()
    {
        //_animatorUI.SetTrigger(_appearAnim);
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void EnterMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
