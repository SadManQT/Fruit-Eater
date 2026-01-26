using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[System.Serializable]
public struct Skin
{
    public string skinName;
    public int skinPrice;
    public bool unlocked;
}

public class UI_SkinSelection : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;
    [Space]

    private DefaultInputActions defaultInput;
    private UI_LevelSelection levelSelectionUI;
    private UI_MainMenu mainMenuUI;

    [Header("UI Skin Details")]
    [SerializeField] private Skin[] skinList;
    private int currentSkinIndex;
    private List<int> skinIndex;

    private int maxPlayerIndex;
    private int currentPlayerIndex;

    [Header("UI details")]
    [SerializeField] private Animator skinDisplay;
    [SerializeField] private TextMeshProUGUI buySelectText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI bankText;
    [SerializeField] private TextMeshProUGUI playerNumberText;

    [Space]
    [SerializeField] private float inputCooldown = .1f;
    private float lastTimeInput;
    private void Awake()
    {
        ResetSkinChoices();
        LoadSkinUnlocks();
        UpdateSkinDisplay();

        mainMenuUI = GetComponentInParent<UI_MainMenu>();
        levelSelectionUI = mainMenuUI.GetComponentInChildren<UI_LevelSelection>(true);
        defaultInput = new DefaultInputActions();
    }

    private void OnEnable()
    {
        ResetSkinChoices();
        UpdateSkinDisplay();

        defaultInput.Enable();
        mainMenuUI.UpdateLastSelected(firstSelected);
        EventSystem.current.SetSelectedGameObject(firstSelected);

        defaultInput.UI.Navigate.performed += ctx => SwitchSkinWithNavigation(ctx);
 
    }

    private void OnDisable()
    {
        defaultInput.Disable();
        defaultInput.UI.Navigate.performed -= ctx => SwitchSkinWithNavigation(ctx);
    }

    private void SwitchSkinWithNavigation(InputAction.CallbackContext ctx)
    {
        if (Time.time - lastTimeInput < inputCooldown)
            return;

        if (ctx.ReadValue<Vector2>().x <= -1)
            PreviousSkin();

        if (ctx.ReadValue<Vector2>().x >= 1)
            NextSkin();
    }

    private void ResetSkinChoices()
    {
        maxPlayerIndex = PlayerManager.instance.maxPlayerCount - 1;
        currentPlayerIndex = 0;
        currentSkinIndex = 0;
        skinIndex = new List<int>();

        for (int i = 0; i < skinList.Length; i++)
        {
            skinIndex.Add(i);
        }
    }

    private void LoadSkinUnlocks()
    {
        for (int i = 0; i < skinList.Length; i++)
        {
            string skinName = skinList[i].skinName;
            bool skinUnlocked = PlayerPrefs.GetInt(skinName + "Unlocked", 0) == 1;

            if(skinUnlocked || i == 0)
                skinList[i].unlocked = true;
        }
    }

    public void SelectSkinButton()
    {
        if (skinList[currentSkinIndex].unlocked == false)
            BuySkin(currentSkinIndex);
        else
        {
            SelectSkin();
        }

        AudioManager.instance.PlaySFX(4);

        UpdateSkinDisplay();
    }

    private void SelectSkin()
    {
        int selectedSkinIndex = skinIndex[currentSkinIndex];

        if (currentPlayerIndex < maxPlayerIndex)
        {
            SkinManager.instance.SetSkinId(selectedSkinIndex, currentPlayerIndex);
            skinIndex.Remove(currentSkinIndex);
            currentPlayerIndex++;
        }
        else
        {
            SkinManager.instance.SetSkinId(selectedSkinIndex, currentPlayerIndex);
            mainMenuUI.SwitchUI(levelSelectionUI.gameObject);
        }
    }

    public void NextSkin()
    {
        lastTimeInput = Time.time;
        currentSkinIndex++;

        if (currentSkinIndex > skinIndex.Count - 1)
            currentSkinIndex = 0;

        AudioManager.instance.PlaySFX(4);

        UpdateSkinDisplay();
    }

    public void PreviousSkin()
    {
        lastTimeInput = Time.time;
        currentSkinIndex--;

        if (currentSkinIndex < 0)
            currentSkinIndex = skinIndex.Count - 1;

        AudioManager.instance.PlaySFX(4);

        UpdateSkinDisplay();
    }

    private void UpdateSkinDisplay()
    {
        bankText.text = "Bank: " + FruitsInBank();
        playerNumberText.text = (currentPlayerIndex + 1) + " Player";

        for (int i = 0; i < skinDisplay.layerCount; i++)
        {
            skinDisplay.SetLayerWeight(i, 0);
        }

        int selectedSkinIndex = skinIndex[currentSkinIndex];

        skinDisplay.SetLayerWeight(selectedSkinIndex, 1);


        if (skinList[selectedSkinIndex].unlocked)
        {
            priceText.transform.parent.gameObject.SetActive(false);
            buySelectText.text = "Select";
        }
        else
        {
            priceText.transform.parent.gameObject.SetActive(true);
            priceText.text = "Price: " + skinList[selectedSkinIndex].skinPrice;
            buySelectText.text = "Buy";

        }
    }

    private void BuySkin(int index)
    {
        if (HaveEnoughFruits(skinList[index].skinPrice) == false)
        {
            AudioManager.instance.PlaySFX(6);
            Debug.Log("Not enough fruits");
            return;
        }



        AudioManager.instance.PlaySFX(10);
        string skinName = skinList[currentSkinIndex].skinName;
        skinList[currentSkinIndex].unlocked = true;

        PlayerPrefs.SetInt(skinName + "Unlocked", 1);
    }

    private int FruitsInBank() => PlayerPrefs.GetInt("TotalFruitsAmount");

    private bool HaveEnoughFruits(int price)
    {
        if (FruitsInBank() > price)
        {
            PlayerPrefs.SetInt("TotalFruitsAmount", FruitsInBank() - price);
            return true;
        }

        return false;
    }


}
