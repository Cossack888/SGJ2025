using System;
using UnityEngine;

public class PlayerStatTracker : MonoBehaviour
{

    public event Action LoseGame;
    public event Action CaughtByNet;
    [SerializeField] private float playerMaxHealth;
    [SerializeField] private float playerCurrentHealth;
    [SerializeField] private int playerMaxAmmo;
    [SerializeField] private int playerCurrentAmmo;
    [SerializeField] private float playerMaxSpeed;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float playerRegen;
    [SerializeField] private float playerJumpHeight;
    [SerializeField] private float playerReload;

    private void Awake()
    {
        MenuManager.Instance.SetStatTracker(this);
        LevelManager.Instance.SetStatTracker(this);
    }

    // HEALTH
    public float PlayerHealth
    {
        get { return playerCurrentHealth; }
        set
        {
            playerCurrentHealth = value;
            playerCurrentHealth = Mathf.Clamp(playerCurrentHealth, 0f, ResetHealth);
            LevelManager.Instance.UpdateUI();
            CheckHealth();
        }
    }

    public float ResetHealth
    {
        get { return playerMaxHealth; }
    }

    // AMMO
    public int PlayerAmmo
    {
        get { return playerCurrentAmmo; }
        set { playerCurrentAmmo = value; }
    }

    public int ResetAmmo
    {
        get { return playerMaxAmmo; }
    }

    // SPEED
    public float PlayerSpeed
    {
        get { return playerSpeed; }
        set
        {
            playerSpeed = value;

        }
    }

    public float ResetSpeed
    {
        get { return playerMaxSpeed; }
    }

    // REGEN
    public float PlayerRegen
    {
        get { return playerRegen; }
        set { playerRegen = value; }
    }

    // JUMP HEIGHT
    public float PlayerJumpHeight
    {
        get { return playerJumpHeight; }
        set { playerJumpHeight = value; }
    }

    // RELOAD SPEED
    public float PlayerReloadSpeed
    {
        get { return playerReload; }
        set { playerReload = value; }
    }

    public void CheckHealth()
    {
        if (playerCurrentHealth <= 0)
        {
            // Essentially invokes the script across the game
            // and any 'listener' set up for LoseGame will process
            // once this script triggers as part of the "set health" process
            LoseGame?.Invoke();
        }
    }
}
