using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Mortar : BaseEnemy
{
    [SerializeField] float waitTime;
    [SerializeField] float randomize;
    [SerializeField] List<SpriteRenderer> listOfRadiuses = new();

    protected override void Awake()
    {
        base.Awake();
        foreach (SpriteRenderer next in listOfRadiuses)
            next.gameObject.SetActive(false);
    }

    protected override void ShootBullet()
    {
        foreach (SpriteRenderer next in listOfRadiuses)
        {
            Vector2 randomPosition = new(
                Random.Range(Player.instance.transform.position.x - randomize, Player.instance.transform.position.x + randomize),
                Random.Range(Player.instance.transform.position.y - randomize, Player.instance.transform.position.y + randomize));

            next.transform.position = randomPosition;
            next.gameObject.SetActive(true);
            StartCoroutine(Activation(next));
        }

        IEnumerator Activation(SpriteRenderer next)
        {
            float elapsedTime = 0f;
            while (elapsedTime < waitTime)
            {
                elapsedTime += Time.deltaTime;
                SetAlpha(next, elapsedTime / waitTime);
                yield return null;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(next.transform.position, 0.9f);
            foreach (Collider2D nextCollider in colliders)
            {
                if (nextCollider.gameObject == Player.instance.gameObject)
                    Player.instance.TakeDamage();
            }
            next.gameObject.SetActive(false);
        }
    }
}
