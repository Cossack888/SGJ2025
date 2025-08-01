using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private InputSystem_Actions _actions;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsInteracting { get; private set; }

    private void Awake()
    {
        _actions = new InputSystem_Actions();

        _actions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        _actions.Player.Move.canceled += _ => MoveInput = Vector2.zero;

        _actions.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        _actions.Player.Look.canceled += _ => LookInput = Vector2.zero;

        _actions.Player.Jump.performed += _ => IsJumping = true;
        _actions.Player.Jump.canceled += _ => IsJumping = false;

        _actions.Player.Crouch.performed += _ => IsCrouching = true;
        _actions.Player.Crouch.canceled += _ => IsCrouching = false;

        _actions.Player.Sprint.performed += _ => IsSprinting = true;
        _actions.Player.Sprint.canceled += _ => IsSprinting = false;

        _actions.Player.Attack.performed += _ => IsAttacking = true;
        _actions.Player.Attack.canceled += _ => IsAttacking = false;

        _actions.Player.Interact.performed += _ => IsInteracting = true;
        _actions.Player.Interact.canceled += _ => IsInteracting = false;
    }

    private void OnEnable() => _actions.Enable();
    private void OnDisable() => _actions.Disable();
}
