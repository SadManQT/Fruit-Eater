using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_LevelSelection : MonoBehaviour
{
    private UI_MainMenu mainMenuUI;
    [SerializeField] private GameObject firstSelected;

    [SerializeField] private UI_LevelButton buttonPrefab;
    [SerializeField] private Transform buttonsParent;

    [SerializeField] private bool[] levelsUnlocked;

    private void Awake()
    {
        mainMenuUI = GetComponentInParent<UI_MainMenu>();

        LoadLevelsInfo();
        CreateLevelButtons();
    }


    private void OnEnable()
    {
        mainMenuUI.UpdateLastSelected(firstSelected);

        GameObject firstLevelButton = buttonsParent.GetChild(0).gameObject;

        if(firstLevelButton != null )
            EventSystem.current.SetSelectedGameObject(firstLevelButton);
        else
            EventSystem.current.SetSelectedGameObject(firstSelected);
    }

   
    private void CreateLevelButtons()
    {
        int levelsAmount = SceneManager.sceneCountInBuildSettings - 1;

        for (int i = 1; i < levelsAmount; i++)
        {
            if (IsLevelUnlocked(i) == false)
                return;

            UI_LevelButton newButton = Instantiate(buttonPrefab, buttonsParent);
            newButton.SetupButton(i);
        }
    }

    private bool IsLevelUnlocked(int levelIndex) => levelsUnlocked[levelIndex];

    private void LoadLevelsInfo()
    {
        int levelsAmount = SceneManager.sceneCountInBuildSettings - 1;

        levelsUnlocked = new bool[levelsAmount];

        for (int i = 1; i < levelsAmount; i++)
        {
            bool levelUnlocked = PlayerPrefs.GetInt("Level" + i + "Unlocked", 0) == 1;

            if (levelUnlocked)
                levelsUnlocked[i] = true;
        }

        levelsUnlocked[1] = true;
    }
}
