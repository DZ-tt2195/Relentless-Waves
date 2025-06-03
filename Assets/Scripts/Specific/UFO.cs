using System.Linq;
using UnityEngine;

public class UFO : BaseEnemy
{
    [SerializeField] float avoid;

    protected override void DamageEffect()
    {
        Vector2 randomPosition = Vector2.zero;
        while (true)
        {
            randomPosition = new(Random.Range(WaveManager.minX + avoid, WaveManager.maxX - avoid), Random.Range(WaveManager.minY / 3f, WaveManager.maxY - avoid));
            Collider2D[] colliders = Physics2D.OverlapCircleAll(randomPosition, avoid);
            bool hasEntityGameComponent = colliders.Any(collider => collider.GetComponent<Entity>() != null);

            if (!hasEntityGameComponent)
            {
                this.transform.position = randomPosition;
                break;
            }
        }
    }
}