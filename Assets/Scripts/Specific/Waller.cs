using UnityEngine;

public class Waller : BaseEnemy
{
    GameObject wall;
    [SerializeField] float wallTimer;

    protected override void Awake()
    {
        base.Awake();
        wall = transform.Find("Wall").gameObject;
        InvokeRepeating(nameof(FlickerWall), wallTimer, wallTimer);
    }

    void FlickerWall()
    {
        wall.SetActive(!wall.activeSelf);
    }
}
