using UnityEngine;

public class DirectProjectile : Projectile
{
    [SerializeField] float _speed;
    protected override void Initialize() { }
    void Update()
    {
        transform.Translate(Vector2.right * _speed * Time.deltaTime);
    }
}
