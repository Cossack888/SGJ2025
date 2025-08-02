using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject smallMonkeyPrefab;
    public GameObject largeMonkeyPrefab;

    private bool player1Assigned = false;
    private bool player2Assigned = false;

    private Gamepad pad1;
    private Gamepad pad2;

    private HashSet<InputDevice> usedDevices = new HashSet<InputDevice>();
    private CameraController cam;
    private void Start()
    {
        cam = FindAnyObjectByType<CameraController>();
    }
    void Update()
    {
        if (!player1Assigned)
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame && !usedDevices.Contains(Keyboard.current))
            {
                SpawnPlayer1WithKeyboard();
                usedDevices.Add(Keyboard.current);
            }
            else if (Gamepad.all.Count > 0)
            {
                foreach (var pad in Gamepad.all)
                {
                    if (pad.buttonSouth.wasPressedThisFrame && !usedDevices.Contains(pad))
                    {
                        SpawnPlayer1WithGamepad(pad);
                        usedDevices.Add(pad);
                        break;
                    }
                }
            }
        }
        else if (!player2Assigned)
        {
            if (Gamepad.all.Count > 0)
            {
                foreach (var pad in Gamepad.all)
                {
                    if (pad.buttonSouth.wasPressedThisFrame && !usedDevices.Contains(pad))
                    {
                        SpawnPlayer2WithGamepad(pad);
                        usedDevices.Add(pad);
                        break;
                    }
                }
            }
        }
    }

    void SpawnPlayer1WithKeyboard()
    {
        var player1 = Instantiate(largeMonkeyPrefab, transform.position + new Vector3(-2, 0, 0), Quaternion.identity);
        var input1 = player1.GetComponent<PlayerInput>();
        input1.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);

        Debug.Log("Gracz 1: Klawiatura");
        player1Assigned = true;
        pad1 = null;
        cam.SetTarget(input1.transform);

    }

    void SpawnPlayer1WithGamepad(Gamepad pad)
    {
        var player1 = Instantiate(largeMonkeyPrefab, transform.position + new Vector3(-2, 0, 0), Quaternion.identity);
        var input1 = player1.GetComponent<PlayerInput>();
        input1.SwitchCurrentControlScheme("Gamepad", pad);

        Debug.Log("Gracz 1: Gamepad " + pad.deviceId);
        player1Assigned = true;
        pad1 = pad;
        cam.SetTarget(input1.transform);
    }

    void SpawnPlayer2WithGamepad(Gamepad pad)
    {
        var player2 = Instantiate(smallMonkeyPrefab, transform.position + new Vector3(2, 0, 0), Quaternion.identity);
        var input2 = player2.GetComponent<PlayerInput>();
        input2.SwitchCurrentControlScheme("Gamepad", pad);

        Debug.Log("Gracz 2: Gamepad " + pad.deviceId);
        player2Assigned = true;
        pad2 = pad;
    }
}
