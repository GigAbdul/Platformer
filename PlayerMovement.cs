using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f;  // Скорость движения
    public float jumpForce = 7f;  // Сила прыжка
    public float gravityScale = 2f; // Гравитация

    [Header("Хитбокс")]
    public Vector2 standingColliderSize = new Vector2(1f, 2f); // Размер хитбокса в стоячем положении
    public Vector2 crouchingColliderSize = new Vector2(1f, 1f); // Размер хитбокса в приседе
    // Нижняя граница хитбокса. При изменении размера нижняя точка останется на этом значении.
    public float hitboxBottom = -1f; 

    [Header("Компоненты")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private bool isGrounded = false;
    private bool isCrouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb.gravityScale = gravityScale;

        // Устанавливаем исходный размер и смещение хитбокса для стоячего состояния
        boxCollider.size = standingColliderSize;
        // Вычисляем смещение так, чтобы нижняя граница хитбокса была равна hitboxBottom:
        boxCollider.offset = new Vector2(0f, hitboxBottom + standingColliderSize.y / 2f);
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (!isCrouching)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y); // Блокировка движения при приседе
        }

        // Поворот персонажа в сторону движения
        if (moveInput > 0)
            spriteRenderer.flipX = false;
        else if (moveInput < 0)
            spriteRenderer.flipX = true;

        // Проверка прыжка
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        // Обработка приседа
        if (Input.GetKeyDown(KeyCode.S))
            Crouch(true);
        if (Input.GetKeyUp(KeyCode.S))
            Crouch(false);

        UpdateAnimation(moveInput);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animator.SetTrigger("JumpTrigger");
        isGrounded = false;
    }

    void Crouch(bool state)
    {
        isCrouching = state;
        animator.SetBool("IsCrouching", state);

        if (isCrouching)
        {
            // При приседе меняем размер хитбокса и вычисляем смещение так, чтобы нижняя точка оставалась на hitboxBottom
            boxCollider.size = crouchingColliderSize;
            boxCollider.offset = new Vector2(0f, hitboxBottom + crouchingColliderSize.y / 2f);
        }
        else
        {
            // Восстанавливаем размеры для стоячего состояния
            boxCollider.size = standingColliderSize;
            boxCollider.offset = new Vector2(0f, hitboxBottom + standingColliderSize.y / 2f);
            animator.SetTrigger("CrouchToIdle");
        }
    }

    void UpdateAnimation(float moveInput)
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.ResetTrigger("JumpTrigger");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
