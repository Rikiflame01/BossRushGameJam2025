using UnityEngine;

public class ProjectileCollisionHandler : MonoBehaviour
{
    private Collider2D projectileCollider;

    private void Start()
    {
        projectileCollider = GetComponent<Collider2D>();

        if (projectileCollider == null)
        {
            Debug.LogError("No Collider2D found on the projectile. Please add one.");
            return;
        }

        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            if (col.CompareTag("Player"))
                continue;
            Physics2D.IgnoreCollision(projectileCollider, col);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Hit the Player!");
            //Apply damage here

            Destroy(gameObject);
        }
    }
}
