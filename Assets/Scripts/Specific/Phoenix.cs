using UnityEngine;
using System.Collections;
using TMPro;

public class Phoenix : BaseEnemy
{
    [SerializeField] float respawnTime;
    [SerializeField] TMP_Text textBox;

    protected override void Awake()
    {
        base.Awake();
        respawnTime *= 2 - PlayerPrefs.GetFloat("Difficulty");
        textBox.gameObject.SetActive(false);
    }

    protected override void DeathEffect()
    {
        base.DeathEffect();
        StartCoroutine(Revive());

        IEnumerator Revive()
        {
            float timer = respawnTime;
            while (timer > 0)
            {
                textBox.gameObject.SetActive(true);
                timer -= Time.deltaTime;
                textBox.text = $"{timer:F1}";
                yield return null;
            }

            health = maxHealth;
            immune = false;

            textBox.gameObject.SetActive(false);
            crossedOut.SetActive(false);
            SetAlpha(this.spriteRenderer, 1f);
        }
    }
}
