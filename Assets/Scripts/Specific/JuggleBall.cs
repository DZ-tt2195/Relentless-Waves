using UnityEngine;

public class JuggleBall : MonoBehaviour
{
    float currentSpeed = -1.25f;

    private void Awake()
    {
        currentSpeed *= PrefManager.GetDifficulty();
    }

    private void Update()
    {
        this.transform.Translate(new Vector2(0, currentSpeed) * Time.deltaTime);
        if (this.transform.position.y < WaveManager.minY)
        {
            Player.instance.TakeDamage();
            this.transform.position = new(this.transform.position.x, WaveManager.maxY);
            currentSpeed = -1 * Mathf.Abs(currentSpeed);
        }
        else if (this.transform.position.y > WaveManager.maxY)
        {
            currentSpeed = -1*Mathf.Abs(currentSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            currentSpeed = Mathf.Abs(currentSpeed);
    }
}
