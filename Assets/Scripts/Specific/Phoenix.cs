using UnityEngine;
using System.Collections;

public class Phoenix : BaseEnemy
{
    [SerializeField] float respawnTime;

    protected override void Awake()
    {
        base.Awake();
        if (PlayerPrefs.GetInt("Hard Mode") == 1)
            respawnTime *= (3 / 4f);
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
            SetAlpha(1f);
        }
    }
}
