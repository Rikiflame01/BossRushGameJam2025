/*
 * Author: Thoring
 * Desc: simple fade script
 * Note: I did not disable the canvas or image when the fade out ends
 */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        FadeOut();
    }
    public void FadeIn()
    {
        StartCoroutine(FadeImage(0, 1));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeImage(1, 0));
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = image.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            image.color = color;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }
}
