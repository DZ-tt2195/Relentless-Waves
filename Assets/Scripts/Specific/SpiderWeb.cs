using UnityEngine;

public class SpiderWeb : Bullet
{
    [SerializeField] float targetXpand;
    [SerializeField] float travelTime;
    [SerializeField] float vanishTime;

    float myTimer = 0;
    bool moving = true;

    void Awake()
    {
        vanishTime *= 2 - PlayerPrefs.GetFloat("Difficulty");
    }

    public override void AssignInfo(float speed, Vector3 direction, Entity owner)
    {
        base.AssignInfo(speed, direction, owner);
        this.transform.localScale = Vector3.zero;
        this.transform.eulerAngles = new(0, 0, Random.Range(-360f, 360f));

        moving = true;
        myTimer = 0;
    }

    protected override void Movement()
    {
        myTimer += Time.deltaTime;

        if (moving)
        {
            base.Movement();
            this.transform.localScale = Vector2.Lerp(Vector3.zero, new(targetXpand, 0.5f), myTimer / travelTime);

            if (myTimer >= travelTime)
            {
                myTimer = 0f;
                moving = false;
            }
        }
        else
        {
            if (myTimer >= vanishTime)
                TryAndReturn(false);
        }
    }
}
