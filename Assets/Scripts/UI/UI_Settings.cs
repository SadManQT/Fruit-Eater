using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float mixerMultiplier = 25;

    [Header("SFX Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxSliderText;
    [SerializeField] private string sfxParametr;

    [Header("BGM Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmSliderText;
    [SerializeField] private string bgmParametr;

    public void SFXSliderValue(float value)
    {
        sfxSliderText.text = Mathf.RoundToInt(value * 100) + "%";
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(sfxParametr, newValue);
    }

    public void BGMSliderValue(float value)
    {
        bgmSliderText.text = Mathf.RoundToInt(value * 100) + "%";
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(bgmParametr, newValue);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(sfxParametr, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParametr,bgmSlider.value);
    }

    private void OnEnable()
    {
        GetComponentInParent<UI_MainMenu>().UpdateLastSelected(firstSelected);
        EventSystem.current.SetSelectedGameObject(firstSelected);

        sfxSlider.value = PlayerPrefs.GetFloat(sfxParametr, .7f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParametr, .7f);
    }
}
