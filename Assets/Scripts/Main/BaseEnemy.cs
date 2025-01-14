using MyBox;
using UnityEngine;

public class BaseEnemy : Entity
{
    [Foldout("Enemy info", true)]
    protected GameObject crossedOut { get; private set; }
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float attackRate;
    [SerializeField] bool lookAtPlayer = true;
    protected Vector3 moveDirection;

    public void EnemySetup()
    {
        this.tag = "Enemy";
        crossedOut = transform.Find("X").gameObject;
        crossedOut.SetActive(false);

        attackRate *= (2-PlayerPrefs.GetFloat("Difficulty"));
        bulletSpeed *= PlayerPrefs.GetFloat("Difficulty");
        moveSpeed *= PlayerPrefs.GetFloat("Difficulty");

        InvokeRepeating(nameof(ShootBullet), attackRate*0.5f, attackRate);
    }

    protected virtual void ShootBullet()
    {
        Vector2 target = AimAtPlayer();
        target.Normalize();
        CreateBullet(prefab, this.transform.position, bulletSpeed, target);
    }

    protected virtual void Update()
    {
        this.transform.Translate(moveSpeed * Time.deltaTime * moveDirection);
        if (lookAtPlayer)
        {
            Vector2 aimDirection = AimAtPlayer();
            spriteRenderer.transform.localEulerAngles = new(0, 0, Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + 90);
        }
    }

    protected Vector2 AimAtPlayer()
    {
        return (Player.instance.transform.position - this.transform.position).normalized;
    }

    protected override void DeathEffect()
    {
        immune = true;
        crossedOut.SetActive(true);
        SetAlpha(this.spriteRenderer, 0.5f);
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