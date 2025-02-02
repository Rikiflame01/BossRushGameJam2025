using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public SoundMenuManager soundMenuManager;
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            {
                soundMenuManager.PlaySFX("Play");
                SceneManager.LoadScene(sceneName);
            }
        }
        else
        {
            Debug.LogError("Scene name is not set");
        }
    }

}
