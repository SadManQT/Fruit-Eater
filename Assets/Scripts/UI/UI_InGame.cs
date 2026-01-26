using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    private PlayerInputSet playerInput;
    private List<Player> playerList;
    public static UI_InGame instance;
    public UI_FadeEffect fadeEffect { get; private set; } // read-only

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI fruitText;
    [SerializeField] private TextMeshProUGUI lifePointsText;

    [SerializeField] private GameObject pauseUI;
    private bool isPaused;

    private void Awake()
    {
        instance = this;

        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
        playerInput = new PlayerInputSet();
    }

    private void OnEnable()
    {
        if (playerInput == null)
            playerInput = new PlayerInputSet();

        try
        {
            if (playerInput == null)
            {
                Debug.LogWarning("UI_InGame.OnEnable: playerInput is null");
                return;
            }

            // Access UI actions safely and log what's missing
            try
            {
                var uiActions = playerInput.UI;
                var pauseAction = uiActions.Pause;
                var navigateAction = uiActions.Navigate;

                // Register callbacks only when actions are present
                pauseAction.performed += ctx => PauseButton();
                navigateAction.performed += ctx => UpdateSelected();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"UI_InGame.OnEnable: failed to access UI actions: {ex.Message}");
                return;
            }

            playerInput.Enable();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"UI_InGame.OnEnable exception: {ex} - playerInput={(playerInput==null?"null":"ok")}");
        }
    }

    private void OnDisable()
    {
        if (playerInput == null)
            return;

        try
        {
            try
            {
                var uiActions = playerInput.UI;
                var pauseAction = uiActions.Pause;
                var navigateAction = uiActions.Navigate;

                pauseAction.performed -= ctx => PauseButton();
                navigateAction.performed -= ctx => UpdateSelected();
            }
            catch { }

            playerInput.Disable();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"UI_InGame.OnDisable exception: {ex.Message}");
        }
    }

    private void Start()
    {
        fadeEffect.ScreenFade(0, 1);
        GameObject pressJoinText = FindFirstObjectByType<UI_TextBlinkEffect>().gameObject;
        PlayerManager.instance.objectsToDisable.Add(pressJoinText);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            PauseButton();
    }

    private void UpdateSelected()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void PauseButton()
    {
        playerList = PlayerManager.instance.GetPlayerList();

        if (isPaused)
            UnpauseTheGame();
        else
            PauseTheGame();
    }

    private void PauseTheGame()
    {
        foreach (var player in playerList)
        {
            player.playerInput.Disable();
        }


        EventSystem.current.SetSelectedGameObject(firstSelected);
        isPaused = true;
        Time.timeScale = 0;
        pauseUI.SetActive(true);
    }

    private void UnpauseTheGame()
    {
        foreach (var player in playerList)
        {
            player.playerInput.Enable();
        }

        isPaused = false;
        Time.timeScale = 1;
        pauseUI.SetActive(false);
    }

    public void GoToMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void UpdateFruitUI(int collectedFruits, int totalFruits)
    {
        if (fruitText != null)
            fruitText.text = collectedFruits + "/" + totalFruits;
    }

    public void UpdateTimerUI(float timer)
    {
        if (timerText != null)
            timerText.text = timer.ToString("00") + " s";
    }

    public void UpdateLifePointsUI(int lifePoints, int maxLifePoints)
    {
        if (lifePointsText == null)
            return;

        if (DifficultyManager.instance.difficulty == DifficultyType.Easy)
        {
            if (lifePointsText.transform != null && lifePointsText.transform.parent != null)
                lifePointsText.transform.parent.gameObject.SetActive(false);
            return;
        }

        lifePointsText.text = lifePoints + "/" + maxLifePoints;
    }
}
