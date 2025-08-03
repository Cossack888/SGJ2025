using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    private InputActionMap _map;

    public event Action AttackPressed;
    public event Action AttackReleased;
    public event Action InteractPressed;
    public event Action InteractReleased;
    public event Action JumpPressed;
    public event Action Menu;
    public event Action MenuClose;

    public event Action NavigateUp;    // dla Previous
    public event Action NavigateDown;  // dla Next
    public event Action MenuSubmit;    // np. Interact
    public event Action MenuCancel;    // np. Close
    public event Action PressA;
    public event Action PressB;
    public event Action PressX;
    public event Action PressY;
    public event Action<bool> ReleaseFromNet;
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsInteracting { get; private set; }

    private void Awake()
    {
        var input = GetComponent<PlayerInput>();
        _map = input.actions.FindActionMap("Player");

        // Ruch i patrzenie
        _map.FindAction("Move").performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        _map.FindAction("Move").canceled += _ => MoveInput = Vector2.zero;

        _map.FindAction("Look").performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        _map.FindAction("Look").canceled += _ => LookInput = Vector2.zero;

        // Skok, sprint, kucanie
        _map.FindAction("Jump").performed += _ => IsJumping = true;
        _map.FindAction("Jump").canceled += _ => IsJumping = false;
        _map.FindAction("Jump").performed += _ => JumpPressed?.Invoke();

        _map.FindAction("Crouch").performed += _ => IsCrouching = true;
        _map.FindAction("Crouch").canceled += _ => IsCrouching = false;

        _map.FindAction("Sprint").performed += _ => IsSprinting = true;
        _map.FindAction("Sprint").canceled += _ => IsSprinting = false;

        // Atak
        _map.FindAction("Attack").started += _ =>
        {
            IsAttacking = true;
            AttackPressed?.Invoke();
        };
        _map.FindAction("Attack").canceled += _ =>
        {
            IsAttacking = false;
            AttackReleased?.Invoke();
        };

        // Interakcja
        _map.FindAction("Interact").started += _ =>
        {
            IsInteracting = true;
            InteractPressed?.Invoke();
            MenuSubmit?.Invoke();
        };
        _map.FindAction("Interact").canceled += _ =>
        {
            IsInteracting = false;
            InteractReleased?.Invoke();
        };

        // Menu
        _map.FindAction("Menu").performed += _ => Menu?.Invoke();
        _map.FindAction("Close").performed += _ =>
        {
            MenuClose?.Invoke();
            MenuCancel?.Invoke();
        };

        // Nawigacja menu
        _map.FindAction("Previous").performed += _ => NavigateUp?.Invoke();
        _map.FindAction("Next").performed += _ => NavigateDown?.Invoke();

        _map.FindAction("PressA").performed += _ => PressA?.Invoke();
        _map.FindAction("PressB").performed += _ => PressB?.Invoke();
        _map.FindAction("PressX").performed += _ => PressX?.Invoke();
        _map.FindAction("PressY").performed += _ => PressY?.Invoke();
    }
    public void ReleasefromNet()
    {
        ReleaseFromNet?.Invoke(true);
    }
}
