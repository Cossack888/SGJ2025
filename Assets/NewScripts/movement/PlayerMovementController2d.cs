using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController2D : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputBindingListSO inputBindings;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground & Wall Check")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float wallCheckHeightOffset = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private Transform groundChecker;

    private Rigidbody2D rb;
    private MovementState2D currentState;
    private RaycastEnvironmentChecker2D environmentChecker;

    public InputBindingListSO InputBindings => inputBindings;
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public Rigidbody2D Rigidbody => rb;
    public Vector2 CurrentMoveInput { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        environmentChecker = new RaycastEnvironmentChecker2D(
            transform,
            groundChecker,
            groundCheckDistance,
            wallCheckHeightOffset,
            groundLayer,
            climbableLayer
        );
    }

    private void OnEnable()
    {
        inputActions.Enable();
        BindInput();
    }

    private void OnDisable()
    {
        inputActions.Disable();
        UnbindInput();
    }

    private void Start()
    {
        foreach (var binding in inputBindings.bindings)
        {
            binding.response?.SetupFromRuntimeAsset(inputActions);
        }

        ChangeState(new GroundedState2D(this));
    }

    private void Update()
    {
        currentState?.Tick();
    }

    private void FixedUpdate()
    {
        currentState?.FixedTick();
    }

    public void ChangeState(MovementState2D newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public bool IsGrounded() => environmentChecker.IsGrounded();
    public bool IsTouchingWallLeft() => environmentChecker.IsTouchingWallLeft();
    public bool IsTouchingWallRight() => environmentChecker.IsTouchingWallRight();

    public void SetMoveInput(Vector2 value)
    {
        CurrentMoveInput = value;
    }

    public void ResetMoveInput()
    {
        CurrentMoveInput = Vector2.zero;
    }

    private void BindInput()
    {
        var moveInput = InputBindings.GetResponse(InputActionType.Player_Move);
        if (moveInput == null) return;

        moveInput.Initialize();
        moveInput.OnPerformed += OnMovePerformed;
        moveInput.OnCanceled += OnMoveCanceled;
    }

    private void UnbindInput()
    {
        var moveInput = InputBindings.GetResponse(InputActionType.Player_Move);
        if (moveInput == null) return;

        moveInput.OnPerformed -= OnMovePerformed;
        moveInput.OnCanceled -= OnMoveCanceled;
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        SetMoveInput(ctx.ReadValue<Vector2>());
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        ResetMoveInput();
    }
}
