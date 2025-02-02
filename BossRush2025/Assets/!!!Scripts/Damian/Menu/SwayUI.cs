using UnityEngine;
using UnityEngine.UI;

public class SwayUI : MonoBehaviour
{
    [Header("Sway Settings")]
    [Tooltip("How quickly the sway motion completes one cycle.")]
    [SerializeField] private float swaySpeed = 1.0f;
    
    [Tooltip("How far the UI element moves or rotates from its initial position.")]
    [SerializeField] private float swayAmount = 10.0f;
    
    [Tooltip("Choose whether the sway goes horizontally, vertically, or rotates.")]
    [SerializeField] private SwayDirection swayDirection = SwayDirection.Horizontal;

    [Header("Offset Settings")]
    [Tooltip("Offset in seconds to shift the sine wave timing (so multiple elements don't sway in sync).")]
    [SerializeField] private float timingOffset = 0f;
    
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        float time = Time.time + timingOffset;
        
        float swayValue = Mathf.Sin(time * swaySpeed) * swayAmount;

        switch (swayDirection)
        {
            case SwayDirection.Horizontal:
                transform.localPosition = new Vector3(
                    initialPosition.x + swayValue,
                    initialPosition.y,
                    initialPosition.z
                );
                break;

            case SwayDirection.Vertical:
                transform.localPosition = new Vector3(
                    initialPosition.x,
                    initialPosition.y + swayValue,
                    initialPosition.z
                );
                break;
            
            case SwayDirection.Rotate:
                transform.localRotation = Quaternion.Euler(
                    initialRotation.eulerAngles.x,
                    initialRotation.eulerAngles.y,
                    initialRotation.eulerAngles.z + swayValue
                );
                break;
        }
    }

    private enum SwayDirection
    {
        Horizontal,
        Vertical,
        Rotate
    }
}
