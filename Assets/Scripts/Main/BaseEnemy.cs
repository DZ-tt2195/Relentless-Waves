using MyBox;
using UnityEngine;

public class BaseEnemy : Entity
{
    [Foldout("Enemy info", true)]
    GameObject crossedOut;
    [SerializeField] protected float moveSpeed;
    [SerializeField] float attackRate;
    protected Vector3 moveDirection;

    public void EnemySetup()
    {
        this.Setup(0f, "Enemy");
        crossedOut = transform.Find("X").gameObject;
        crossedOut.SetActive(false);
        if (PlayerPrefs.GetInt("Hard Mode") == 1)
        {
            attackRate *= (3/4f);
            bulletSpeed *= (4/3f);
            moveSpeed *= (4/3f);
        }
        InvokeRepeating(nameof(ShootBullet), attackRate, attackRate);
    }

    protected virtual void ShootBullet()
    {
        Vector2 target = AimAtPlayer();
        target.Normalize();
        CreateBullet(prefab, bulletSpeed, this.transform.position, target);
    }

    protected virtual void Update()
    {
        this.transform.Translate(moveSpeed * Time.deltaTime * moveDirection);
        Vector2 aimDirection = AimAtPlayer();
        spriteRenderer.transform.localEulerAngles = new(0, 0, Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + 90);
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

    public void OnDestroy()
    {
        while (bulletQueue.Count > 0)
        {
            Bullet nextBullet = bulletQueue.Dequeue();
            if (nextBullet != null)
                Destroy(nextBullet.gameObject);
        }
    }
}