using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float airControlMultiplier = 0.5f; // Control en el aire (50% del normal)

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Llamado por PlayerInput
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    void Update()
    {
        CheckGround();
    }

    void FixedUpdate()
    {
        // Determinar velocidad según si está en el suelo o en el aire
        float currentSpeed = isGrounded ? moveSpeed : moveSpeed * airControlMultiplier;
        
        float targetX = transform.position.x + moveInput * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(new Vector2(targetX, rb.position.y));
    }

    void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            groundCheckPoint.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        isGrounded = hit.collider != null;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(
            groundCheckPoint.position,
            groundCheckPoint.position + Vector3.down * groundCheckDistance
        );
    }

    // Getter público para saber si está en el suelo (útil para animaciones)
    public bool IsGrounded() => isGrounded;
}
