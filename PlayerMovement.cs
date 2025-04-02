using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f;      // Скорость движения
    public float jumpForce = 7f;      // Сила прыжка
    public float gravityScale = 2f;   // Масштаб гравитации

    [Header("Хитбокс")]
    public Vector2 standingColliderSize = new Vector2(1f, 2f); // Размер хитбокса в стоячем положении
    public Vector2 crouchingColliderSize = new Vector2(1f, 1f);  // Размер хитбокса в приседе

    [Header("Компоненты")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private bool isGrounded = false;
    private bool isCrouching = false;

    // Переменная для хранения нижней границы хитбокса (в локальных координатах)
    private float colliderBottom;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        rb.gravityScale = gravityScale;

        // Инициализируем хитбокс для стоячего состояния и сохраняем его нижнюю границу.
        boxCollider.size = standingColliderSize;
        // Вычисляем нижнюю границу хитбокса:
        // (offset.y - size.y/2) дает позицию нижней границы относительно объекта.
        colliderBottom = boxCollider.offset.y - boxCollider.size.y / 2f;
        // Обновляем offset так, чтобы нижняя граница осталась на месте:
        boxCollider.offset = new Vector2(boxCollider.offset.x, colliderBottom + standingColliderSize.y / 2f);
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Если не приседаем, разрешаем движение; иначе горизонтальное движение блокируется.
        if (!isCrouching)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        // Поворот персонажа в сторону движения
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        // Прыжок, если на земле
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Обработка приседа (клавиша "S")
        if (Input.GetKeyDown(KeyCode.S))
        {
            Crouch(true);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            Crouch(false);
        }

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
            // При приседе меняем размер хитбокса и вычисляем offset так,
            // чтобы нижняя граница оставалась неизменной.
            boxCollider.size = crouchingColliderSize;
            boxCollider.offset = new Vector2(boxCollider.offset.x, colliderBottom + crouchingColliderSize.y / 2f);
        }
        else
        {
            // Восстанавливаем размеры и offset для стоячего состояния.
            boxCollider.size = standingColliderSize;
            boxCollider.offset = new Vector2(boxCollider.offset.x, colliderBottom + standingColliderSize.y / 2f);
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
        {
            isGrounded = false;
        }
    }
}
