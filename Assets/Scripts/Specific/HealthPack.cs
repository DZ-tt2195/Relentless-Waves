using UnityEngine;

public class HealthPack : MonoBehaviour
{
    private void Update()
    {
        this.transform.Translate(new Vector2(0, -1f) * Time.deltaTime);
        if (this.transform.position.y < WaveManager.minY)
            Destroy(this.gameObject);
    }
}
