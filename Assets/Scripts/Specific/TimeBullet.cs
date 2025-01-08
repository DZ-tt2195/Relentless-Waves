using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TimeBullet : Bullet
{
    [SerializeField] float timer;

    public override void AssignInfo(float speed, Vector3 direction, Entity owner)
    {
        base.AssignInfo(speed, direction, owner);
        StartCoroutine(Disappear());

        IEnumerator Disappear()
        {
            yield return new WaitForSeconds(timer);
            TryAndReturn(false);
        }
    }

    protected override void Movement()
    {
        this.transform.Translate(bulletSpeed * Time.deltaTime * (Player.instance.transform.position - this.transform.position).normalized);
    }

}
