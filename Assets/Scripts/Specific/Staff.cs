using UnityEngine;

public class Staff : BaseEnemy
{
    GameObject wall;
    Star starPrefab;
    [SerializeField] float bulletOffset;
    [SerializeField] int numBullets;

    protected override void Awake()
    {
        base.Awake();
        wall = transform.Find("Wall").gameObject;

        starPrefab = transform.Find("Star").GetComponent<Star>();
        starPrefab.gameObject.SetActive(false);
    }

    protected override void ShootBullet()
    {
        Vector2 target = AimAtPlayer();
        target.Normalize();

        for (int i = 0; i < numBullets; i++)
            CreateBullet(bulletPrefab, this.transform.position, bulletSpeed, new(target.x + RandomOffSet(), target.y + RandomOffSet()));

        if (health > 0 && !starPrefab.gameObject.activeSelf)
        {
            starPrefab.transform.position = this.transform.position;
            starPrefab.tag = this.tag;
            starPrefab.AssignInfo(bulletSpeed, new(target.x + RandomOffSet(), target.y + RandomOffSet()), this);
        }

        float RandomOffSet()
        {
            return Random.Range(-bulletOffset, bulletOffset);
        }
    }

    protected override void DeathEffect()
    {
        base.DeathEffect();
        wall.SetActive(false);
    }
}
