using System;
using UnityEngine;

public class TsukuyomiBoss : MonoBehaviour
{
    public static event Action TsukuyomiTeleport;
    //TsukuyomiTeleport?.Invoke(); to trigger the attack
    public static event Action TsukuyomiAllRoundFire;
    //TsukuyomiAllRoundFire?.Invoke(); to trigger the attack

    void Awake()
    {
        
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        
    }
}
