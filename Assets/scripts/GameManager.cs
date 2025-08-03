using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject largeMonkeyPrefab;

    private bool player1Assigned = false;
    private bool player2Assigned = false;

    private HashSet<InputDevice> usedDevices = new HashSet<InputDevice>();
    private CameraController cam;

    private GameObject largeMonkeyInstance;
    private GameObject smallMonkeyInstance;
    private MinigameController minigameController;
    private void Start()
    {
        cam = FindAnyObjectByType<CameraController>();
        minigameController = FindFirstObjectByType<MinigameController>();

        // Instantiate the large monkey
        largeMonkeyInstance = Instantiate(largeMonkeyPrefab, transform.position + new Vector3(-2, 0, 0), Quaternion.identity);

        // Find the small monkey as a child of the large monkey (including inactive objects)
        var smallController = largeMonkeyInstance.GetComponentInChildren<SmallMonkeyController>(true);

        if (smallController == null)
        {
            Debug.LogError("Could not find SmallMonkeyController as a child of the large monkey!");
            return;
        }

        smallMonkeyInstance = smallController.gameObject;

        // Temporarily disable PlayerInput for the small monkey
        var smallInput = smallMonkeyInstance.GetComponent<PlayerInput>();
        if (smallInput != null)
            smallInput.enabled = false;
        else
            Debug.LogError("Missing PlayerInput component on the small monkey!");
    }
    void Update()
    {
        if (!player1Assigned)
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame && !usedDevices.Contains(Keyboard.current))
            {
                AssignLargeMonkeyToDevice(Keyboard.current, "Keyboard&Mouse");
            }
            else
            {
                foreach (var pad in Gamepad.all)
                {
                    if (pad.buttonSouth.wasPressedThisFrame && !usedDevices.Contains(pad))
                    {
                        AssignLargeMonkeyToDevice(pad, "Gamepad");
                        break;
                    }
                }
            }
        }
        else if (!player2Assigned)
        {
            foreach (var pad in Gamepad.all)
            {
                if (pad.buttonSouth.wasPressedThisFrame && !usedDevices.Contains(pad))
                {
                    AssignSmallMonkeyToDevice(pad, "Gamepad");
                    break;
                }
            }
        }
    }

    private void AssignLargeMonkeyToDevice(InputDevice device, string controlScheme)
    {
        var input = largeMonkeyInstance.GetComponent<PlayerInput>();
        input.SwitchCurrentControlScheme(controlScheme, device);
        input.ActivateInput();
        minigameController.RegisterAs1Player(input.GetComponent<PlayerInputHandler>());
        usedDevices.Add(device);
        player1Assigned = true;

        cam.SetTarget(input.transform);
        Debug.Log("Player 1 controls the large monkey (" + controlScheme + ")");
        MenuManager.Instance.AssignControllingInput(input.GetComponent<PlayerInputHandler>());
    }

    private void AssignSmallMonkeyToDevice(InputDevice device, string controlScheme)
    {
        if (smallMonkeyInstance == null)
        {
            Debug.LogError("Small monkey instance is missing!");
            return;
        }

        var input = smallMonkeyInstance.GetComponent<PlayerInput>();

        if (!input.enabled)
            input.enabled = true;

        input.SwitchCurrentControlScheme(controlScheme, new InputDevice[] { device });
        input.ActivateInput();
        minigameController.RegisterAs2Player(input.GetComponent<PlayerInputHandler>());
        usedDevices.Add(device);
        player2Assigned = true;

        Debug.Log("Player 2 controls the small monkey (" + controlScheme + ")");
        MenuManager.Instance.AssignControllingInput(input.GetComponent<PlayerInputHandler>());
        if (player1Assigned && player2Assigned && LevelManager.Instance != null)
        {
            LevelManager.Instance.StartGame();
        }
    }
}
