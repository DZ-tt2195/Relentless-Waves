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
        waitTime *= 2 - PlayerPrefs.GetFloat("Difficulty");
        foreach (SpriteRenderer next in listOfRadiuses)
            next.gameObject.SetActive(false);
    }

    protected override void ShootBullet()
    {
        Vector2 playerPosition = Player.instance.transform.position;
        foreach (SpriteRenderer next in listOfRadiuses)
        {
            next.transform.position = new(playerPosition.x + RandomOffSet(), playerPosition.y + RandomOffSet());
            next.gameObject.SetActive(true);
            StartCoroutine(Activation(next));
        }

        float RandomOffSet()
        {
            return Random.Range(-randomize, randomize);
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
