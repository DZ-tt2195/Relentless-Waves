using TMPro;
using UnityEngine;

public class Lightbulb : BaseEnemy
{
    [SerializeField] float blackOutTime;

    protected override void Awake()
    {
        base.Awake();
        blackOutTime *= PlayerPrefs.GetFloat("Difficulty");
    }

    protected override void DamageEffect()
    {
        base.DamageEffect();
        Player.instance.blackOutTime += this.blackOutTime;
    }

    protected override void DeathEffect()
    {
        base.DeathEffect();
        Player.instance.blackOutTime += this.blackOutTime;
    }
}
