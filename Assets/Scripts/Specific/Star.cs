using UnityEngine;

public class Star : Bullet
{
    protected override void TryAndReturn(bool landed)
    {
        this.gameObject.SetActive(false);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player target))
        {
            this.owner.TakeDamage();
            TryAndReturn(true);
        }
        else if (collision.CompareTag("Wall"))
        {
            if (collision.TryGetComponent(out Bullet bullet) && !bullet.owner.CompareTag(this.owner.tag))
                TryAndReturn(false);
            else if (!collision.transform.parent.CompareTag(owner.tag))
                TryAndReturn(false);
        }
    }
}