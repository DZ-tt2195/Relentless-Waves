using UnityEngine;

public class Resupply : MonoBehaviour
{
    private void Update()
    {
        this.transform.Translate(new Vector2(0, -1.5f) * Time.deltaTime);
        if (this.transform.position.y < WaveManager.minY)
            WaveManager.instance.ReturnResupply(this);
    }
}