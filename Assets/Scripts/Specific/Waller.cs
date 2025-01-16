using UnityEngine;

public class Waller : BaseEnemy
{
    GameObject wall;
    [SerializeField] float wallOff;
    [SerializeField] float wallOn;
    float timer;

    protected override void Awake()
    {
        base.Awake();
        wall = transform.Find("Wall").gameObject;
        wallOn *= PlayerPrefs.GetFloat("Difficulty");
        wallOff *= 2 - PlayerPrefs.GetFloat("Difficulty");

        wall.SetActive(true);
        timer = wallOn;
    }

    protected override void Update()
    {
        base.Update();
        timer -= Time.deltaTime;

        if (timer < 0f)
        {
            if (wall.activeSelf)
            {
                timer = wallOff;
                wall.SetActive(false);
            }
            else
            {
                timer = wallOn;
                wall.SetActive(true);
            }
        }
    }
}
