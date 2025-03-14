using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "ScripatableObjects/EnemySO")]
public class EnemySO : ScriptableObject
{
    public string _name;
    public float _health;
    public float _damage;
    public float _speed;
    public float _attackInterval;
    public float _minStopRadius;
}
