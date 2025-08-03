using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    public enum ButtonType { down, right, left, up }

    [Header("Players")]
    public PlayerInputHandler player1;
    public PlayerInputHandler player2;

    [Header("UI")]
    public TMP_Text promptText;
    public TMP_Text scoreText;
    public GameObject UI;

    [Header("Settings")]
    public float timeBetweenPrompts = 2f;
    public int scoreToWin = 10;
    private Action pressA1, pressB1, pressX1, pressY1;
    private Action pressA2, pressB2, pressX2, pressY2;
    private ButtonType currentPrompt;
    private int currentScore = 0;
    public bool gameActive;
    private bool p1Pressed, p2Pressed;

    private void OnEnable()
    {
        UI.SetActive(false);
    }
    public void RegisterAs1Player(PlayerInputHandler input)
    {
        player1 = input;

    }
    public void RegisterAs2Player(PlayerInputHandler input)
    {
        player2 = input;

    }

    public void ActivateGame()
    {
        if (gameActive) { return; }
        gameActive = true;
        UI.SetActive(true);
        RegisterEvents(player1);
        RegisterEvents(player2);
        currentScore = 0;
        UpdateUI();
        SetNextPrompt();
    }
    private void SetNextPrompt()
    {
        p1Pressed = false;
        p2Pressed = false;
        currentPrompt = (ButtonType)UnityEngine.Random.Range(0, 4);
        promptText.text = $"Naciœnij {currentPrompt}";
    }
    private void OnDisable()
    {
        UnregisterEvents(player1);
        UnregisterEvents(player2);
    }

    private void RegisterEvents(PlayerInputHandler input)
    {
        if (input != null)
        {
            if (input == player1)
            {
                pressA1 = () => OnButtonPressed(ButtonType.down, input);
                pressB1 = () => OnButtonPressed(ButtonType.right, input);
                pressX1 = () => OnButtonPressed(ButtonType.left, input);
                pressY1 = () => OnButtonPressed(ButtonType.up, input);

                input.PressA += pressA1;
                input.PressB += pressB1;
                input.PressX += pressX1;
                input.PressY += pressY1;
            }
            else if (input == player2)
            {
                pressA2 = () => OnButtonPressed(ButtonType.down, input);
                pressB2 = () => OnButtonPressed(ButtonType.right, input);
                pressX2 = () => OnButtonPressed(ButtonType.left, input);
                pressY2 = () => OnButtonPressed(ButtonType.up, input);

                input.PressA += pressA2;
                input.PressB += pressB2;
                input.PressX += pressX2;
                input.PressY += pressY2;
            }
        }
    }

    private void UnregisterEvents(PlayerInputHandler input)
    {
        if (input != null)
        {
            if (input == player1)
            {
                input.PressA -= pressA1;
                input.PressB -= pressB1;
                input.PressX -= pressX1;
                input.PressY -= pressY1;
            }
            else if (input == player2)
            {
                input.PressA -= pressA2;
                input.PressB -= pressB2;
                input.PressX -= pressX2;
                input.PressY -= pressY2;
            }
        }
    }
    private void OnButtonPressed(ButtonType pressedButton, PlayerInputHandler who)
    {
        if (pressedButton != currentPrompt)
            return;

        if (who == player1)
            p1Pressed = true;
        else if (who == player2)
            p2Pressed = true;

        if (p1Pressed && p2Pressed)
        {
            currentScore++;
            UpdateUI();

            if (currentScore >= scoreToWin)
            {
                promptText.text = "";
                player1.ReleasefromNet();
                player2.ReleasefromNet();
                UI.SetActive(false);
            }
            else
            {
                SetNextPrompt(); // natychmiast nowe has³o
            }
        }
    }

    private IEnumerator NextPromptRoutine()
    {
        UpdateUI();
        p1Pressed = false;
        p2Pressed = false;
        yield return new WaitForSeconds(timeBetweenPrompts);

        currentPrompt = (ButtonType)UnityEngine.Random.Range(0, 4);
        promptText.text = $"Naciï¿½nij {currentPrompt}";
    }

    private void UpdateUI()
    {
        scoreText.text = $"Punkty: {currentScore}/{scoreToWin}";
    }
}
