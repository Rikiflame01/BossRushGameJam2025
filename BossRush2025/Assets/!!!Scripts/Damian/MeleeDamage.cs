using UnityEngine;
using System.Collections;

public class MeleeDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float damageInterval = 1.5f;
    [SerializeField] private string playerTag = "Player";

    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            HealthManager healthManager = other.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                damageCoroutine = StartCoroutine(DealDamageOverTime(healthManager));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DealDamageOverTime(HealthManager healthManager)
    {
        while (true)
        {
            Debug.Log("Dealing " + damageAmount + " to Player");
            healthManager.TakeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
