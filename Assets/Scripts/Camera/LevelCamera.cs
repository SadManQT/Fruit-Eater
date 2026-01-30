using Unity.Cinemachine;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    private CinemachineCamera cinemachine;

    private void Awake()
    {
        cinemachine = GetComponentInChildren<CinemachineCamera>(true);
        EnableCamera(false);
    }

    public void EnableCamera(bool enable)
    {
        cinemachine.gameObject.SetActive(enable);
    }
    public void SetNewTarger(Transform newTarget)
    {
        cinemachine.Follow = newTarget;
    }
}
