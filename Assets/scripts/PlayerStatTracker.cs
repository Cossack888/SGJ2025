using UnityEngine;

public class PlayerStatTracker : MonoBehaviour
{
    [SerializeField] private float playerHealth;
    [SerializeField] private float playerCurrentHealth;
    [SerializeField] private int playerMaxAmmo;
    [SerializeField] private int playerCurrentAmmo;
    [SerializeField] private float playerMaxSpeed;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float playerRegen;
    [SerializeField] private float playerJumpHeight;
    [SerializeField] private float playerReload;

    // HEALTH
    public float PlayerHealth
    {
        get { return playerCurrentHealth; }
        set { playerCurrentHealth = value; }
    }

    public float ResetHealth
    {
        set { playerCurrentHealth = playerHealth; }
    }

    // AMMO
    public int PlayerAmmo
    {
        get { return playerCurrentAmmo; }
        set { playerCurrentAmmo = value; }
    }

    public int ResetAmmo
    {
        set { playerCurrentAmmo = playerMaxAmmo; }
    }

    // SPEED
    public float PlayerSpeed
    {
        get { return playerSpeed; }
        set { playerSpeed = value; }
    }

    public float ResetSpeed
    {
        set { playerSpeed = playerMaxSpeed; }
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
}
