using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStat", menuName = "ScriptableObjects/EnemyStat")]
public class EnemyStat : ScriptableObject
{
    public Sprite sprite;
    public int health;
    public Color bulletColor;
    public Vector2 bulletSize;
    public bool customTarget;
    [ConditionalField(nameof(customTarget), inverse: false)] public Vector2 aim;
    public float attackRate;
    public float bulletSpeed;
}
