/*
 * AUthor:Thorin
 * Description: sample boss that inherits the entity state abstract class
 * intention implement custom logic here.
 */
using UnityEngine;

public class Susano : EntityState
{
    protected override void HandleIdle() { Debug.Log("Enemy B Idle Behavior"); }
    protected override void HandleWalking() { Debug.Log("Enemy B Walking Behavior"); }
    protected override void HandleRunning() { Debug.Log("Enemy B Running Behavior"); }
}
