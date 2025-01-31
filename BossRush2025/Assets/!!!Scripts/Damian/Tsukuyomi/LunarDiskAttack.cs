using UnityEngine;
using System.Collections;

public class LunarDiskAttack : MonoBehaviour
{
    [Header("Disk Setup")]
    [SerializeField] private GameObject lunarDiskPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Disk Configuration")]
    public float diskLifetime = 5f;
    [SerializeField] private float diskSpeed = 2f;
    [SerializeField] private float spinSpeed = 50f;

    [Header("Projectile Configuration")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 4f;
    [SerializeField] private float projectileLifetime = 3f;

    public int repeatTimes = 1;
    private bool isDiskActive = false;
    private bool isPaused = false;

    private GameObject firstCurrentDisk;
    private GameObject secondCurrentDisk;

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
            bool isVertical = Random.Range(0, 2) == 0;
            for (int i = 0; i < repeatTimes; i++) 
            {
                LunarDiskBehaviour diskBehavior;
                if (isVertical) 
                {
                    firstCurrentDisk = Instantiate(lunarDiskPrefab, spawnPoint.position, Quaternion.identity);
                    diskBehavior = firstCurrentDisk.GetComponent<LunarDiskBehaviour>();
                }
                else
                {
                    secondCurrentDisk = Instantiate(lunarDiskPrefab, spawnPoint.position, Quaternion.identity);
                    diskBehavior = secondCurrentDisk.GetComponent<LunarDiskBehaviour>();
                }
                if (diskBehavior != null)
                {
                    diskBehavior.Initialize(
                        diskSpeed,
                        spinSpeed,
                        diskLifetime,
                        projectilePrefab,
                        projectileSpeed,
                        projectileLifetime,
                        isVertical
                    );
                }
                isVertical = !isVertical;
            }
        }
    }

    private void ResetAttack()
    {
        isDiskActive = false;
        isPaused = false;

        if (firstCurrentDisk != null)
        {
            Destroy(firstCurrentDisk);
        }
        if (secondCurrentDisk != null)
        {
            Destroy(secondCurrentDisk);
        }
    }
}
