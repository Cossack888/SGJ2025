using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public CameraController cameraController;
    public Transform[] playerTransforms;
    private int activePlayerIndex = 0;

    private void Start()
    {
        cameraController.SetTarget(playerTransforms[0]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            activePlayerIndex = (activePlayerIndex + 1) % playerTransforms.Length;
            cameraController.SetTarget(playerTransforms[activePlayerIndex]);
        }
    }
}