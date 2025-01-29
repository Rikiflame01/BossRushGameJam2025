using UnityEngine;
using System.Collections;

public class LunarDiskAttack : MonoBehaviour
{
    [Header("Disk Setup")]
    [SerializeField] private GameObject lunarDiskPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Disk Configuration")]
    [SerializeField] private float diskLifetime = 5f;
    [SerializeField] private float diskSpeed = 2f;
    [SerializeField] private float spinSpeed = 50f;

    [Header("Projectile Configuration")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 4f;
    [SerializeField] private float projectileLifetime = 3f;

    private bool isDiskActive = false;
    private bool isPaused = false;

    private GameObject currentDisk;

    void Start()
    {
        TsukuyomiBoss._tsukuyomiLunarDiskAttack += HandleLunarDisk;
    }

    void OnDisable()
    {
        TsukuyomiBoss._tsukuyomiLunarDiskAttack -= HandleLunarDisk;
    }

    public void HandleLunarDisk(string command)
    {
        switch (command)
        {
            case "Start":
                if (!isDiskActive)
                {
                    LunarDiskRoutine();
                }
                break;

            case "Stop":
                StopAllCoroutines();
                ResetAttack();
                break;

            case "Pause":
                isPaused = true;
                break;

            case "Resume":
                isPaused = false;
                break;
        }
    }

    private void LunarDiskRoutine()
    {
        isDiskActive = true;
        isPaused = false;

        SpawnLunarDisk();

        isDiskActive = false;
        isPaused = false;
    }

    private void SpawnLunarDisk()
    {
        if (lunarDiskPrefab != null && spawnPoint != null)
        {
            currentDisk = Instantiate(lunarDiskPrefab, spawnPoint.position, Quaternion.identity);

            LunarDiskBehaviour diskBehavior = currentDisk.GetComponent<LunarDiskBehaviour>();
            if (diskBehavior != null)
            {
                diskBehavior.Initialize(
                    diskSpeed,
                    spinSpeed,
                    diskLifetime,
                    projectilePrefab,
                    projectileSpeed,
                    projectileLifetime
                );
            }
        }
    }

    private void ResetAttack()
    {
        isDiskActive = false;
        isPaused = false;

        if (currentDisk != null)
        {
            Destroy(currentDisk);
        }
    }
}
