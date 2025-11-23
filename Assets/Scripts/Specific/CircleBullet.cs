using UnityEngine;

public class CircleBullet : Bullet
{
    float myAngle;
    Vector2 centralPoint;
    float radius;

    protected override void Awake()
    {
        base.Awake();
        disappearOnWall = false;
    }

    public override void AssignInfo(float speed, Vector3 direction, Entity owner)
    {
        base.AssignInfo(speed, direction, owner);
        centralPoint = this.transform.position;
        myAngle = direction.x;
        radius = 4.5f;
        Spin();
    }

    void Spin()
    {
        float rad = myAngle * Mathf.Deg2Rad;
        float x = centralPoint.x + Mathf.Cos(rad) * radius;
        float y = centralPoint.y + Mathf.Sin(rad) * radius;
        this.transform.position = new Vector3(x, y, 0);
    }

    protected override void Movement()
    {
        myAngle += (bulletSpeed*Mathf.Rad2Deg)*Time.deltaTime;
        radius -= bulletSpeed*Time.deltaTime;
        Spin();

        if (radius <= 0f)
            TryAndReturn(false);
    }
}
