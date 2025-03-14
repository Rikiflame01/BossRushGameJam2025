using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class Intro : MonoBehaviour
{
    [SerializeField] private float _durationOfIntro = 7f;
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private GameObject _introBackground;

    void Start()
    {
        _introBackground.SetActive(false);
        StartCoroutine(IntroDuration());
    }

    private IEnumerator IntroDuration()
    {
        yield return new WaitForSeconds(_durationOfIntro);
        gameObject.SetActive(false);
        _videoPlayer.gameObject.SetActive(false);
    }
}
