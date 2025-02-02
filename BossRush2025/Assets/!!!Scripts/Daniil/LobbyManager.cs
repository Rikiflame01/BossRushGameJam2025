using UnityEngine;
using System.Collections;

public class LobbyManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(StartMenu());
    }
    private IEnumerator StartMenu()
    {
        yield return new WaitForSeconds(6f);
        AudioManager._instance.PlayBGM("Main menu");
    }
}
