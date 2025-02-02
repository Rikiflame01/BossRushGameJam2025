using UnityEngine;
using System.Collections.Generic;

public class BossesButtons : MonoBehaviour
{
    [SerializeField] private List<GameObject> _bossesButtons;

    void Start()
    {
        for (int i = 0; PlayerPrefs.HasKey("Boss " + (i + 1)); i++)
        {
            _bossesButtons[i].SetActive(true);
        }
    }
}
