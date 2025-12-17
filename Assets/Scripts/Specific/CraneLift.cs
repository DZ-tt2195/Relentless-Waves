using UnityEngine;

public class CraneLift : MonoBehaviour
{
    [SerializeField] Transform wallToMove;
    [SerializeField] float moveUp;
    [SerializeField] float moveDown;

    [SerializeField] BaseEnemy thisEnemy;
    bool inContact = false;
    LineRenderer lineRender;

    void Awake()
    {
        moveUp *= PrefManager.GetDifficulty();
        lineRender = GetComponent<LineRenderer>();
        lineRender.startWidth = 0.1f;
        lineRender.endWidth = 0.1f;
        lineRender.positionCount = 2;
    }

    void Update()
    {
        Vector3 moveWall = new Vector3(thisEnemy.transform.position.x, 0, 0);
        if (inContact)
            moveWall.y = Mathf.MoveTowards(wallToMove.position.y, WaveManager.minY, moveDown*Time.deltaTime);
        else
            moveWall.y = Mathf.MoveTowards(wallToMove.position.y, thisEnemy.transform.position.y-0.5f, moveUp*Time.deltaTime);
        wallToMove.transform.position = moveWall;

        lineRender.SetPosition(0, thisEnemy.transform.position + new Vector3(0.3f, 0.25f));
        lineRender.SetPosition(1, wallToMove.transform.position + new Vector3(0.3f, 0.1f));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == Player.instance.gameObject)
            inContact = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == Player.instance.gameObject)
            inContact = false;
    }
}
