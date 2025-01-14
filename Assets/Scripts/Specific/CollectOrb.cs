using UnityEngine;

public class CollectOrb : BaseEnemy
{
    GameObject wall;
    Orb orbPrefab;
    [SerializeField] float bulletOffset;
    [SerializeField] int numBullets;

    protected override void Awake()
    {
        base.Awake();
        wall = transform.Find("Wall").gameObject;

        orbPrefab = transform.Find("Orb").GetComponent<Orb>();
        orbPrefab.gameObject.SetActive(false);
    }

    protected override void ShootBullet()
    {
        Vector2 target = AimAtPlayer();
        target.Normalize();

        for (int i = 0; i < numBullets; i++)
            CreateBullet(prefab, this.transform.position, bulletSpeed, new(target.x + RandomOffSet(), target.y + RandomOffSet()));

        if (health > 0 && !orbPrefab.gameObject.activeSelf)
        {
            orbPrefab.transform.position = this.transform.position;
            orbPrefab.tag = this.tag;
            orbPrefab.AssignInfo(bulletSpeed, new(target.x + RandomOffSet(), target.y + RandomOffSet()), this);
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
