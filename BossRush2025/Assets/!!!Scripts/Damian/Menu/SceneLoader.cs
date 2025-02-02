using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public SoundMenuManager soundMenuManager;
    private int _startScene = 0;
    public void LoadScene()
    {
        StartCoroutine(LoadSceneWithDelay());
    }

    private IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(1);

        soundMenuManager.PlaySFX("Play");
        _startScene = PlayerPrefs.GetInt("StartScene", _startScene);
        if (_startScene == 0)
        {
            PlayerPrefs.SetInt("StartScene", 1);
            SceneManager.LoadScene("DummyScene");
        }
        else
        {
            SceneManager.LoadScene("GameplayScene");
        }
    }
}
