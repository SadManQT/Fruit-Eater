using UnityEngine;

public class LevelCameraView : MonoBehaviour
{
    private LevelCamera levelCamera;
    private int playersInView;

    private void Awake()
    {
        levelCamera = GetComponentInParent<LevelCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            playersInView++;

            if (playersInView >= levelCamera.playerList.Count)
            {
                levelCamera.ChangeCameraLensSize(levelCamera.minCameraSize);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            playersInView--;

            if (playersInView < levelCamera.playerList.Count)
            {
                levelCamera.ChangeCameraLensSize(levelCamera.maxCameraSize);
            }
        }
    }
}
