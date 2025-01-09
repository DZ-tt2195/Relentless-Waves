using UnityEngine;

public class Waller : BaseEnemy
{
    GameObject wall;
    [SerializeField] float wallTimer;

    protected override void Awake()
    {
        base.Awake();
        wall = transform.Find("Wall").gameObject;
        if (PlayerPrefs.GetInt("Hard Mode") == 1)
            wallTimer *= (4 / 3f);
        InvokeRepeating(nameof(FlickerWall), wallTimer, wallTimer);
    }

    void FlickerWall()
    {
        wall.SetActive(!wall.activeSelf);
    }
}
