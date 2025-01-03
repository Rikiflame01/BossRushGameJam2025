using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    [SerializeField] float _speed;
    void Update()
    {
        transform.Translate(Vector2.right * _speed * Time.deltaTime);
    }
}
