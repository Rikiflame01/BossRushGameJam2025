using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeAmplitude = 0.1f;
    [SerializeField] private float shakeFrequency = 10f;   

    [Header("Grow Settings")]
    [SerializeField] private float growScale = 1.2f;       
    [SerializeField] private float growSpeed = 5f;          

    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Coroutine> shakeCoroutines = new Dictionary<GameObject, Coroutine>();

    private void Start()
    {
        GameObject[] buttonObjects = GameObject.FindGameObjectsWithTag("Button");

        foreach (GameObject buttonObj in buttonObjects)
        {
            originalPositions[buttonObj] = buttonObj.transform.localPosition;
            originalScales[buttonObj] = buttonObj.transform.localScale;

            EventTrigger trigger = buttonObj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = buttonObj.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => { OnHoverEnter(buttonObj); });
            trigger.triggers.Add(enterEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { OnHoverExit(buttonObj); });
            trigger.triggers.Add(exitEntry);
        }
    }
    private void OnHoverEnter(GameObject buttonObj)
    {
        if (shakeCoroutines.ContainsKey(buttonObj) && shakeCoroutines[buttonObj] != null)
        {
            StopCoroutine(shakeCoroutines[buttonObj]);
        }

        shakeCoroutines[buttonObj] = StartCoroutine(ShakeAndGrow(buttonObj));
    }
    private void OnHoverExit(GameObject buttonObj)
    {
        if (shakeCoroutines.ContainsKey(buttonObj) && shakeCoroutines[buttonObj] != null)
        {
            StopCoroutine(shakeCoroutines[buttonObj]);
            shakeCoroutines[buttonObj] = null;
        }

        StartCoroutine(RestoreOriginalTransform(buttonObj));
    }

    private IEnumerator ShakeAndGrow(GameObject buttonObj)
    {
        Transform btnTransform = buttonObj.transform;
        Vector3 startPos = originalPositions[buttonObj];
        Vector3 originalScale = originalScales[buttonObj];
        Vector3 targetScale = originalScale * growScale;

        while (true)
        {
            btnTransform.localScale = Vector3.Lerp(btnTransform.localScale, targetScale, Time.deltaTime * growSpeed);

            float offsetX = Mathf.Sin(Time.time * shakeFrequency) * shakeAmplitude;
            float offsetY = Mathf.Cos(Time.time * shakeFrequency) * shakeAmplitude;

            btnTransform.localPosition = startPos + new Vector3(offsetX, offsetY, 0);

            yield return null;
        }
    }

    private IEnumerator RestoreOriginalTransform(GameObject buttonObj)
    {
        Transform btnTransform = buttonObj.transform;
        Vector3 startPos = btnTransform.localPosition;
        Vector3 endPos = originalPositions[buttonObj];

        Vector3 startScale = btnTransform.localScale;
        Vector3 endScale = originalScales[buttonObj];

        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            btnTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
            btnTransform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        btnTransform.localPosition = endPos;
        btnTransform.localScale = endScale;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

}
