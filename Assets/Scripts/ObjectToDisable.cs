using UnityEngine;

public class ObjectToDisable : MonoBehaviour
{
    private void Start()
    {
        PlayerManager.instance.objectsToDisable.Add(this.gameObject);
    }
}
