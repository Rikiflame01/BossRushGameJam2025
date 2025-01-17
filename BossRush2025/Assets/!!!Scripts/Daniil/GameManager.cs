using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _ritualCircle;
    public float RitualCircleRadius;
    public Transform RitualCenter;
    public event Action RitualStart, RitualFinished;
    public void RitualBegin()
    {
        RitualStart?.Invoke();
        _ritualCircle.SetActive(true);
    }
    public void RitualEnd(int numberCircle)
    {
        RitualFinished?.Invoke();
        _ritualCircle.SetActive(false);
    }
}
