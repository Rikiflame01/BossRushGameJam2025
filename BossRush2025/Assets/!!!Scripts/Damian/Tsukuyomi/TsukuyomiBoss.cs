using System;
using UnityEngine;

public class TsukuyomiBoss : EntityState
{
    public static event Action TsukuyomiTeleport;
    //TsukuyomiTeleport?.Invoke(); to trigger the attack
    public static event Action TsukuyomiAllRoundFire;
    //TsukuyomiAllRoundFire?.Invoke(); to trigger the attack

    public static event Action _tsukuyomiCrescentAttack;
    public static event Action _tsukuyomiSwordStrikeAttack;
    protected override void HandleIdle() { Debug.Log("Enemy B Idle Behavior"); }
    protected override void HandleWalking()
    {
    }
    protected override void HandleRunning() { Debug.Log("Enemy B Running Behavior"); }
    protected override void Initialize()
    {
    }
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
