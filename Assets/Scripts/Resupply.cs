using UnityEngine;

namespace Week1
{
    public class Resupply : MonoBehaviour
    {
        float minY;

        private void Awake()
        {
            float cameraHeight = 2f * Player.instance.mainCamera.orthographicSize;
            minY = Player.instance.mainCamera.transform.position.y - cameraHeight / 2f;
        }

        private void Update()
        {
            this.transform.Translate(new Vector2(0, -1.5f) * Time.deltaTime);
            if (this.transform.position.y < minY)
                WaveManager.instance.ReturnResupply(this);
        }
    }
}