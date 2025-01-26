using System;
using UnityEngine;

public class TsukuyomiBoss : MonoBehaviour
{
    public static event Action TsukuyomiTeleport;
    //TsukuyomiTeleport?.Invoke(); to trigger the attack
    public static event Action TsukuyomiAllRoundFire;
    //TsukuyomiAllRoundFire?.Invoke(); to trigger the attack

    public static event Action _tsukuyomiCrescentAttack;
    public static event Action _tsukuyomiSwordStrikeAttack;

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    void OnDestroy()
    {
        
    }
}
