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

    [Header("Projectile Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

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
        HandleShooting();
        UpdateAnimation();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0; // Flip sprite based on movement direction
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
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
            animator.SetTrigger("Crouch"); // Trigger crouch animation.
        else
            animator.SetTrigger("StandUp"); // Trigger stand-up animation
    }

    void HandleShooting()
    {
        // Only allow shooting when not crouching
        if (isCrouching)
            return;

        if (Input.GetKeyDown(KeyCode.J)) // Shoot key
        {
            Vector2 shootDirection = DetermineShootDirection();
            Shoot(shootDirection);
        }
    }

    Vector2 DetermineShootDirection()
{
    Vector2 direction = Vector2.zero;

    // Check for directions based on keys pressed
    if (Input.GetKey(KeyCode.I)) 
        direction += Vector2.up;
    if (Input.GetKey(KeyCode.J)) 
        direction += Vector2.left;
    if (Input.GetKey(KeyCode.K)) 
        direction += Vector2.down;
    if (Input.GetKey(KeyCode.L)) 
        direction += Vector2.right;

    // Debug to check the direction
    Debug.Log("Shooting Direction: " + direction);
    return direction.normalized;
}

    void Shoot(Vector2 direction)
{
    // Ensure the direction is valid and bulletPrefab is assigned
    if (direction != Vector2.zero && bulletPrefab != null)
    {
        // Debug to verify shoot execution
        Debug.Log("Shooting in direction: " + direction);

        // Set the appropriate shooting animation based on direction
        if (direction == Vector2.up)
            animator.SetTrigger("ShootUp");
        else if (direction == Vector2.down)
            animator.SetTrigger("ShootDown");
        else if (direction == Vector2.left)
            animator.SetTrigger("ShootLeft");
        else if (direction == Vector2.right)
            animator.SetTrigger("ShootRight");

        // Instantiate bullet and set its direction
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.Initialize(direction); // Pass the direction to the bullet
    }
}

    void UpdateAnimation()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y); // For jump/fall animations
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