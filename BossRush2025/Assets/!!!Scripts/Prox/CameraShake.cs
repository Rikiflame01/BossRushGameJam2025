using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Camera Shake Properties")]
    [SerializeField] private float _cameraShakeStrentgh = 0.25f;
    [SerializeField] private float _cameraShakeTime = 0.25f;
    public static CameraShake _instance;

    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Shake()
    {
        Camera.main.transform.DOShakeRotation(_cameraShakeStrentgh, _cameraShakeTime);
    }

    public void Shake(float strength, float time)
    {
        Camera.main.transform.DOShakeRotation(strength, time);
    }

}
