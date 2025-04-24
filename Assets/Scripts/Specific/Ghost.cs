using UnityEngine;

public class Ghost : BaseEnemy
{
    protected override void Awake()
    {
        base.Awake();
        moveDirection = Random.Range(0, 2) == 0 ? Vector3.left : Vector3.right;
    }

    protected override void ShootBullet()
    {
        base.ShootBullet();
        this.SetAlpha(spriteRenderer, 1f);
    }

    protected override void DamageEffect()
    {
        base.DamageEffect();
        this.SetAlpha(spriteRenderer, 1f);
    }

    protected override void Update()
    {
        base.Update();
        if (this.transform.position.x < WaveManager.minX)
            moveDirection = Vector3.right;
        else if (transform.position.x > WaveManager.maxX)
            moveDirection = Vector3.left;

        if (health > 0)
            this.SetAlpha(spriteRenderer, Mathf.Max(0, spriteRenderer.color.a - ((4.75f-attackRate) * Time.deltaTime)));
    }
}
