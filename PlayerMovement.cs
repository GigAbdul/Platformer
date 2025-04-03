using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 7f;
    public float gravityScale = 2f;
    public float jumpCooldown = 0.2f;

    [Header("Hitbox Settings")]
    public Vector2 standingColliderSize = new Vector2(1f, 2f);
    public Vector2 crouchingColliderSize = new Vector2(1f, 1f);

    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private bool isGrounded = false;
    private bool isCrouching = false;
    private bool canJump = true;
    private float colliderBottom;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        rb.gravityScale = gravityScale;
        boxCollider.size = standingColliderSize;
        colliderBottom = boxCollider.offset.y - boxCollider.size.y / 2f;
        boxCollider.offset = new Vector2(boxCollider.offset.x, colliderBottom + standingColliderSize.y / 2f);
    }

    void Update()
    {
        HandleMovement();
        HandleCrouch();
        HandleJump();
        UpdateAnimation();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;
        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);

        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump)
        {
            Jump();
        }
    }

    void Jump()
    {
        canJump = false;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animator.SetTrigger("JumpStart");
        isGrounded = false;
        Invoke("ResetJump", jumpCooldown);
    }

    void ResetJump()
    {
        canJump = true;
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.S))
            Crouch(true);
        if (Input.GetKeyUp(KeyCode.S))
            Crouch(false);
    }

    void Crouch(bool state)
    {
        isCrouching = state;
        animator.SetBool("IsCrouching", state);
        boxCollider.size = state ? crouchingColliderSize : standingColliderSize;
        boxCollider.offset = new Vector2(boxCollider.offset.x, colliderBottom + (state ? crouchingColliderSize.y : standingColliderSize.y) / 2f);

        if (state)
            animator.SetTrigger("Crouch");
        else
            animator.SetTrigger("StandUp");
    }

    void UpdateAnimation()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetFloat("VerticalSpeed", rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetTrigger("JumpLand");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetTrigger("JumpMidAir");
        }
    }
}
