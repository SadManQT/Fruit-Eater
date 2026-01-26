using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [Header("Screen Shake")]
    [SerializeField] private Vector2 shakeVelocity;

    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        instance = this;

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ScreenShake(float shakeDirection)
    {
        impulseSource.DefaultVelocity = new Vector2(shakeVelocity.x * shakeDirection, shakeVelocity.y);
        impulseSource.GenerateImpulse();
    }
}
