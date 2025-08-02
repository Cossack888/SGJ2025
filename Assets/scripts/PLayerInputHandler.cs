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

        _map.FindAction("Move").performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        _map.FindAction("Move").canceled += _ => MoveInput = Vector2.zero;

        _map.FindAction("Look").performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        _map.FindAction("Look").canceled += _ => LookInput = Vector2.zero;

        _map.FindAction("Jump").performed += _ => IsJumping = true;
        _map.FindAction("Jump").canceled += _ => IsJumping = false;

        _map.FindAction("Crouch").performed += _ => IsCrouching = true;
        _map.FindAction("Crouch").canceled += _ => IsCrouching = false;

        _map.FindAction("Sprint").performed += _ => IsSprinting = true;
        _map.FindAction("Sprint").canceled += _ => IsSprinting = false;

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

        _map.FindAction("Interact").started += _ =>
        {
            IsInteracting = true;
            InteractPressed?.Invoke();
        };
        _map.FindAction("Interact").canceled += _ =>
        {
            IsInteracting = false;
            InteractReleased?.Invoke();
        };
    }
}
