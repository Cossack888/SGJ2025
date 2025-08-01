using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody _rb;
    private PlayerInputHandler _input;
    public bool _isGrounded;

    private Vector2 _moveInput;
    private bool _jumpRequested;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _input = FindFirstObjectByType<PlayerInputHandler>();
    }

    private void Update()
    {
        _moveInput = _input.MoveInput;

        if (_input.IsJumping && _isGrounded)
        {
            _jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        CheckGrounded();

        // Movement
        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y);
        move = transform.TransformDirection(move) * moveSpeed;
        Vector3 newVelocity = new Vector3(move.x, _rb.linearVelocity.y, move.z);
        _rb.linearVelocity = newVelocity;

        // Jump
        if (_jumpRequested)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z); // Reset Y before jump
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpRequested = false;
        }
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f, groundLayer);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (groundCheckDistance + 0.1f));
    }
#endif
}
