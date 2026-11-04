using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float acceleration = 18f;
    public float deceleration = 12f;

    public float jumpForce = 7f;
    public float groundCheckDistance = 0.7f;

    private Rigidbody rb;
    private Vector2 movementInput;
    private bool jumpRequested;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        movementInput = Vector2.zero;

        if (Keyboard.current == null) return;

        if (Keyboard.current.wKey.isPressed)
            movementInput.y += 1;

        if (Keyboard.current.sKey.isPressed)
            movementInput.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            movementInput.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            movementInput.x += 1;

        movementInput = Vector2.ClampMagnitude(movementInput, 1f);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        // Raycast downward to check for ground
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        Vector3 currentVelocity = rb.linearVelocity;

        Vector3 horizontalVelocity = new Vector3(
            currentVelocity.x, 0f, currentVelocity.z
        );

        Vector3 targetVelocity = new Vector3(
            movementInput.x * moveSpeed,
            0f,
            movementInput.y * moveSpeed
        );

        float changeSpeed = movementInput.sqrMagnitude > 0
            ? acceleration
            : deceleration;

        Vector3 newHorizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity,
            targetVelocity,
            changeSpeed * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(
            newHorizontalVelocity.x,
            currentVelocity.y,
            newHorizontalVelocity.z
        );

        // Jump only when grounded
        if (jumpRequested && isGrounded)
        {
            rb.AddForce(
                Vector3.up * jumpForce,
                ForceMode.VelocityChange
            );
        }

        jumpRequested = false;
    }

    void OnDrawGizmos()
    {
        // Draw raycast in Scene view
        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.down * groundCheckDistance
        );
    }
}
