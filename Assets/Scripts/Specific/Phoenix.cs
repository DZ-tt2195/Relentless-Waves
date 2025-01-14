using UnityEngine;
using System.Collections;

public class Phoenix : BaseEnemy
{
    [SerializeField] float respawnTime;

    protected override void Awake()
    {
        base.Awake();
        respawnTime *= 2 - PlayerPrefs.GetFloat("Difficulty");
    }

    protected override void DeathEffect()
    {
        base.DeathEffect();
        StartCoroutine(Revive());

        IEnumerator Revive()
        {
            yield return new WaitForSeconds(respawnTime);
            health = maxHealth;
            immune = false;
            crossedOut.SetActive(false);
            SetAlpha(this.spriteRenderer, 1f);
        }
    }
}
