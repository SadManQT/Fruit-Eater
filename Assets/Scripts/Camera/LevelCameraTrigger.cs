using UnityEngine;

public class LevelCameraTrigger : MonoBehaviour
{
    private LevelCamera levelCamera;

    private int playersInTrigger;

    private void Awake()
    {
        levelCamera = GetComponentInParent<LevelCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            playersInTrigger++;

            if (playersInTrigger == levelCamera.playerList.Count)
            {
                levelCamera.EnableCamera(true);
                levelCamera.EnableLimits(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            playersInTrigger--;

            if (playersInTrigger == 0)
            {
                levelCamera.EnableCamera(false);
                levelCamera.EnableLimits(false);
            }
        }
    }
}
