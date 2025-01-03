using UnityEngine;

public class BaseEnemy : Entity
{
    GameObject crossedOut;
    [SerializeField] float attackRate;

    public void EnemySetup()
    {
        this.Setup(0f, "Enemy");
        crossedOut = transform.Find("X").gameObject;
        crossedOut.SetActive(false);
        InvokeRepeating(nameof(ShootBullet), 1f, attackRate);
    }

    protected virtual void ShootBullet()
    {
        Vector2 target = AimAtPlayer();
        target.Normalize();
        CreateBullet(prefab, bulletSpeed, this.transform.position, target);
    }

    protected virtual void Update()
    {
        Vector2 direction = AimAtPlayer();
        spriteRenderer.transform.localEulerAngles = new(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90);
    }

    protected Vector2 AimAtPlayer()
    {
        return (Player.instance.transform.position - this.transform.position).normalized;
    }

    protected override void DeathEffect()
    {
        immune = true;
        crossedOut.SetActive(true);
        SetAlpha(0.5f);
    }
}