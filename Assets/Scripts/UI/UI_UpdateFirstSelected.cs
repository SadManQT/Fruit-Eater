using UnityEngine;
using UnityEngine.EventSystems;

public class UI_UpdateFirstSelected : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}
