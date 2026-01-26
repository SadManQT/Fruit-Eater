using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private HashSet<Player> playersFinished = new HashSet<Player>();

    private bool CanFinishLevel()
    {
        int requiredPlayers = PlayerManager.instance.playerCountWinCondition;
        
        if (playersFinished.Count >= requiredPlayers)
            return true;

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null && !playersFinished.Contains(player))
        {
            playersFinished.Add(player);
            AudioManager.instance.PlaySFX(2);

            anim.SetTrigger("activate");

            if(CanFinishLevel())
                GameManager.instance.LevelFinished();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
            playersFinished.Remove(player);
    }
}
