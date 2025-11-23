using UnityEngine;

public class Knight : BaseEnemy
{
    protected override void ShootBullet()
    {
        CreateBullet(bulletPrefab, this.transform.position, bulletSpeed, Vector3.down);
    }
}
