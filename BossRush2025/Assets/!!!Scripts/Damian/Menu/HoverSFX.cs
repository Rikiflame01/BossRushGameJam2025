using UnityEngine;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler
{
    private GameObject soundObject;
    private SoundMenuManager soundMenuManager;

    private void Awake() {
     soundObject = GameObject.Find("MenuSFXmanager");
     soundMenuManager = soundObject.GetComponent<SoundMenuManager>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        soundMenuManager.PlaySFX("buttonhover");
    }
}