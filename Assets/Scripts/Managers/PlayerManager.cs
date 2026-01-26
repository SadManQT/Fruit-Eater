using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static event Action OnPlayerRespawn;
    public static event Action OnPlayerDeath;

    private LevelSplitscreenSetup splitscreenSetup;

    public PlayerInputManager playerInputManager { get; private set; }
    public static PlayerManager instance;

    public List<GameObject> objectsToDisable;

    public int lifePoints;
    public int maxPlayerCount = 1;
    public int playerCountWinCondition;
    [Header("Player")]
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private string[] playerDevice;

    private void Awake()
    {
        // Get PlayerInputManager early so we can handle duplicates safely
        playerInputManager = GetComponent<PlayerInputManager>();

        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // If another PlayerManager exists, disable this object's PlayerInputManager
            // to avoid multiple PlayerInputManagers and Unity warnings, then destroy
            if (playerInputManager != null)
                playerInputManager.enabled = false;

            Destroy(this.gameObject);
            return;
        }

        // Ensure only one PlayerInputManager is enabled across the whole scene/game.
        // Run during Awake so we disable extras before their OnEnable runs.
        try
        {
            var allManagers = UnityEngine.Object.FindObjectsByType<PlayerInputManager>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None);
            bool keptOne = false;
            foreach (var mgr in allManagers)
            {
                if (!keptOne && mgr.enabled)
                {
                    // prefer the PlayerInputManager attached to this GameObject
                    if (mgr == playerInputManager)
                    {
                        keptOne = true;
                        continue;
                    }
                    // if this object doesn't have one enabled yet, keep the first enabled
                    keptOne = true;
                    continue;
                }

                // disable any additional managers
                if (mgr != playerInputManager && mgr.enabled)
                    mgr.enabled = false;
            }
        }
        catch { }
    }

    private void OnEnable()
    {
        if (playerInputManager == null)
            playerInputManager = GetComponent<PlayerInputManager>();

        if (playerInputManager != null)
        {
            playerInputManager.onPlayerJoined += AddPlayer;
            playerInputManager.onPlayerLeft += RemovePlayer;
        }
    }



    private void OnDisable()
    {
        if (playerInputManager != null)
        {
            playerInputManager.onPlayerJoined -= AddPlayer;
            playerInputManager.onPlayerLeft -= RemovePlayer;
        }
    }

    public void SetupMaxPlayersCount(int newPlayersCount)
    {
        maxPlayerCount = newPlayersCount;
        var t = playerInputManager.GetType();
        var setMethod = t.GetMethod("SetMaximumPlayerCount");
        if (setMethod != null)
            setMethod.Invoke(playerInputManager, new object[] { maxPlayerCount });
        else
        {
            var prop = t.GetProperty("maxPlayerCount") ?? t.GetProperty("maximumPlayerCount") ?? t.GetProperty("maxPlayers") ?? t.GetProperty("maximumPlayers");
            if (prop != null && prop.CanWrite)
                prop.SetValue(playerInputManager, maxPlayerCount);
        }
    }

    public void EnableJoinAndUpdateLifePoints()
    {
        splitscreenSetup = FindFirstObjectByType<LevelSplitscreenSetup>();

        playerInputManager.EnableJoining();
        playerCountWinCondition = maxPlayerCount;
        lifePoints = maxPlayerCount;
        UI_InGame.instance.UpdateLifePointsUI(lifePoints, maxPlayerCount);
    }

    private void AddPlayer(PlayerInput newPlayer)
    {

        Player playerScript = newPlayer.GetComponent<Player>();

        playerList.Add(playerScript);

        OnPlayerRespawn?.Invoke();
        PlaceNewPlayerAtRespawnPoint(newPlayer.transform);

        int newPlayerNumber = GetPlayerNumber(newPlayer);
        int newPlayerSkinId = SkinManager.instance.GetSkinId(newPlayerNumber);

        playerScript.UpdateSkin(newPlayerSkinId);

        foreach (GameObject gameObject in objectsToDisable)
        {
            if (gameObject != null)
                gameObject.SetActive(false);
        }

        if (playerInputManager.splitScreen == true)
        {
            newPlayer.camera = splitscreenSetup.mainCamera[newPlayerNumber];
            splitscreenSetup.cinemachineCamera[newPlayerNumber].Follow = newPlayer.transform;
        }
    }

    private void RemovePlayer(PlayerInput player)
    {
        if (player == null)
            return;

        Player playerScript = player.GetComponent<Player>();
        if (playerScript == null)
            return;

        if (playerList.Contains(playerScript))
            playerList.Remove(playerScript);


        if (CanRemoveLifePoints() && lifePoints > 0)
            lifePoints--;

        if (lifePoints <= 0)
        {
            playerCountWinCondition--;
            playerInputManager.DisableJoining();

            if (playerList.Count <= 0)
                GameManager.instance.RestartLevel();
        }

        if (UI_InGame.instance != null)
            UI_InGame.instance.UpdateLifePointsUI(lifePoints, maxPlayerCount);

        OnPlayerDeath?.Invoke();
    }

    private bool CanRemoveLifePoints()
    {
        if (DifficultyManager.instance.difficulty == DifficultyType.Hard)
        {
            return true;
        }


        if (GameManager.instance.fruitsCollected <= 0 && DifficultyManager.instance.difficulty == DifficultyType.Normal)
        {
            return true;
        }

        return false;
    }

    private int GetPlayerNumber(PlayerInput newPlayer)
    {
        int newPlayerNumber = 0;

        foreach (var device in newPlayer.devices)
        {
            for (int i = 0; i < playerDevice.Length; i++)
            {
                if (playerDevice[i] == "Empty")
                {
                    newPlayerNumber = i;
                    playerDevice[i] = device.name;
                    break;
                }
                else if (playerDevice[i] == device.name)
                {
                    newPlayerNumber = i;
                    break;
                }

            }
        }

        return newPlayerNumber;
    }

    public List<Player> GetPlayerList() => playerList;

    public void UpdateRespawnPosition(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;
    private void PlaceNewPlayerAtRespawnPoint(Transform newPlayer)
    {
        if (respawnPoint == null)
            respawnPoint = FindFirstObjectByType<StartPoint>().transform;


        newPlayer.position = respawnPoint.position;
    }
}
