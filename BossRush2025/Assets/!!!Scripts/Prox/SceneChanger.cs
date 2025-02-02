using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Fade _fade;
    [SerializeField] private float _changeSceneDelay = 1f;

    public void ChangeScene(int sceneIndex)
    {
        StartCoroutine(ChangeSceneWithDelay(sceneIndex));
    }

    private IEnumerator ChangeSceneWithDelay(int sceneIndex)
    {
        _fade.FadeIn();
        yield return new WaitForSecondsRealtime(_changeSceneDelay);
        SceneManager.LoadScene(sceneIndex);
    }
}
