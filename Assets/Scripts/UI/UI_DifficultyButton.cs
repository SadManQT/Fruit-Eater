using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_DifficultyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI difficultyInfo;

    [TextArea]
    [SerializeField] private string description;

   
    public void OnPointerEnter(PointerEventData eventData)
    {
        difficultyInfo.text = description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        difficultyInfo.text = "";
    }

    public void OnSelect(BaseEventData eventData)
    {
        difficultyInfo.text = description;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        difficultyInfo.text = "";
    }
}
