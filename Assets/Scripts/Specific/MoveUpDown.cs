using UnityEngine;

public class MoveUpDown : BaseEnemy
{
    protected override void Awake()
    {
        base.Awake();
        moveDirection = Vector3.down;
    }

    protected override void Update()
    {
        base.Update();
        if (this.transform.position.y < WaveManager.minY)
            moveDirection = Vector3.up;
        else if (transform.position.y > WaveManager.maxY)
            moveDirection = Vector3.down;
    }

    protected override void ShootBullet()
    {
        Vector2 target1 = new(-1, 0);
        CreateBullet(prefab, this.transform.position, bulletSpeed, target1);

        Vector2 target2 = new(1, 0);
        CreateBullet(prefab, this.transform.position, bulletSpeed, target2);
    }
}
