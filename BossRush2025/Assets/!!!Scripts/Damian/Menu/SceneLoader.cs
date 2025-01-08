using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        else
        {
            Debug.LogError("Scene name is not set");
        }
    }

}
