using UnityEngine;
using System.Collections;

public class GravityPullAttack : MonoBehaviour
{
    [SerializeField] private float preparationTime = 1f;
    [SerializeField] private float pullStrength = 5f;
    [SerializeField] private float pullDuration = 3f;

    [SerializeField] private float shakeMagnitude = 0.1f;

    private Transform player;
    private Rigidbody2D playerRb;
    private Movement playerMovement;

    private bool isPulling = false;
    private bool isPaused = false;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerRb = playerObject.GetComponent<Rigidbody2D>();
            playerMovement = playerObject.GetComponent<Movement>();
        }
        TsukuyomiBoss._TsukuyomiGravityPull += HandleGravityPull;
    }

    private void OnDestroy()
    {
        TsukuyomiBoss._TsukuyomiGravityPull -= HandleGravityPull;
    }

    private void HandleGravityPull(string command)
    {
        switch (command)
        {
            case "Start":
                if (!isPulling)
                {
                    if (playerMovement != null)
                        playerMovement.enabled = false;

                    StartCoroutine(GravityPull());
                }
                break;

            case "Stop":
                StopAllCoroutines();
                isPulling = false;
                isPaused = false;

                if (playerMovement != null)
                    playerMovement.enabled = true;
                break;

            case "Pause":
                isPaused = true;
                break;

            case "Resume":
                isPaused = false;
                break;
        }
    }

    private IEnumerator GravityPull()
    {
        if (player == null || playerRb == null) yield break;

        isPulling = true;
        isPaused = false;

        Vector3 originalPos = transform.localPosition;
        float shakeTimer = 0f;

        while (shakeTimer < preparationTime && isPulling)
        {
            while (isPaused)
                yield return null;

            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            shakeTimer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;

        float elapsedTime = 0f;
        while (elapsedTime < pullDuration && isPulling)
        {
            while (isPaused)
                yield return null;

            if (player != null && playerRb != null)
            {
                Vector2 forceDirection = (transform.position - player.position).normalized;
                playerRb.linearVelocity = forceDirection * pullStrength;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;

        isPulling = false;
        isPaused = false;

        if (playerMovement != null)
            playerMovement.enabled = true;
    }
}
