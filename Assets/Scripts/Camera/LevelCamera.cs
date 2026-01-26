using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    public List<Player> playerList { get; private set; }
    private CinemachineCamera cinemachine;


    [SerializeField] private Collider2D[] limits;
    [SerializeField] private Transform cameraCenterPoint;
    public float minCameraSize;
    public float maxCameraSize;
    private float targetLensSize;

    private bool cameraActive;
    private bool cameraFocusedOnCenter;
    private Transform player1;
    private Transform player2;

    private void Awake()
    {
        cinemachine = GetComponentInChildren<CinemachineCamera>(true);
        targetLensSize = cinemachine.Lens.OrthographicSize;
        playerList = new List<Player>();

        EnableLimits(false);
        InvokeRepeating(nameof(UpdateCenterPointPosition), 0, .1f);
        //EnableCamera(false);
    }

    private void Update()
    {

        if (cameraActive == false)
            return;

        UpdateLensSizeIfNeeded();
    }

    private void UpdateCameraStatus()
    {
        playerList = PlayerManager.instance.GetPlayerList();

        if (playerList == null || playerList.Count == 0)
        {
            // No players: stop following anything and disable center focus
            cameraFocusedOnCenter = false;
            SetNewTarget(null);
            player1 = player2 = null;
            return;
        }

        if (playerList.Count > 1)
        {
            SetNewTarget(cameraCenterPoint);

            player1 = playerList[0] != null ? playerList[0].transform : null;
            player2 = playerList[1] != null ? playerList[1].transform : null;
            cameraFocusedOnCenter = (player1 != null && player2 != null);
        }
        else
        {
            cameraFocusedOnCenter = false;
            var p = playerList[0];
            SetNewTarget(p != null ? p.transform : null);
            player1 = player2 = null;
        }
    }

    public void UpdateCenterPointPosition()
    {
        if (cameraActive == false)
            return;

        if (cameraFocusedOnCenter == false)
            return;

        Vector3 midPoint = (player1.position + player2.position) / 2;
        cameraCenterPoint.position = midPoint;
    }

    public void ChangeCameraLensSize(float size)
    {
        targetLensSize = size;
    }

    private void UpdateLensSizeIfNeeded()
    {
        float currentLensSize = cinemachine.Lens.OrthographicSize;

        if (Mathf.Abs(currentLensSize - targetLensSize) > .05f)
        {
            float newLensSize = Mathf.Lerp(currentLensSize, targetLensSize, .5f * Time.deltaTime);
            cinemachine.Lens.OrthographicSize = newLensSize;
        }
        else
            cinemachine.Lens.OrthographicSize = targetLensSize;
    }

    public void EnableCamera(bool enable)
    {
        cameraActive = enable;
        cinemachine.gameObject.SetActive(enable);
    }

    public void EnableLimits(bool enable)
    {
        foreach (var collider in limits)
        {
            collider.enabled = enable;
        }            
    }
    public void SetNewTarget(Transform newTarget)
    {
        cinemachine.Follow = newTarget;
    }

    private void OnEnable()
    {
        PlayerManager.OnPlayerRespawn += UpdateCameraStatus;
        PlayerManager.OnPlayerDeath += UpdateCameraStatus;
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerRespawn -= UpdateCameraStatus;
        PlayerManager.OnPlayerDeath -= UpdateCameraStatus;
    }
}
