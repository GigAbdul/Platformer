using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f;  // Скорость движения
    public float jumpForce = 7f;  // Сила прыжка
    public float gravityScale = 2f; // Гравитация

    [Header("Компоненты")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCollider;  // Для изменения хитбокса

    private bool isGrounded = false;
    private bool isCrouching = false;

    // Ползунок для изменения хитбокса
    public Vector2 standingColliderSize = new Vector2(1f, 2f); // Стандартный размер хитбокса
    public Vector2 crouchingColliderSize = new Vector2(1f, 1f); // Размер хитбокса в сидячем положении

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // Ищем Rigidbody2D
        spriteRenderer = GetComponent<SpriteRenderer>();  // Ищем SpriteRenderer
        animator = GetComponent<Animator>();  // Ищем Animator
        boxCollider = GetComponent<BoxCollider2D>();  // Ищем BoxCollider2D
        rb.gravityScale = gravityScale; // Устанавливаем гравитацию
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal"); // Движение по X

        // Если персонаж не сидит, он может двигаться
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
            spriteRenderer.flipX = false; // Вправо - обычный спрайт
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true; // Влево - отзеркаленный спрайт
        }

        // Проверка прыжка
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Проверка сидения (теперь на "S")
        if (Input.GetKeyDown(KeyCode.S))
        {
            Crouch(true);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            Crouch(false);
        }

        // Выбор анимации
        UpdateAnimation(moveInput);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animator.SetTrigger("JumpTrigger"); // Запуск анимации прыжка
        isGrounded = false; // Отключаем состояние "на земле"
    }

    void Crouch(bool state)
    {
        isCrouching = state;
        animator.SetBool("IsCrouching", state); // Обновляем состояние сидения

        // Проверка, когда персонаж садится или встает
        if (isCrouching)
        {
            boxCollider.size = crouchingColliderSize; // Уменьшаем хитбокс при сидении
            boxCollider.offset = new Vector2(0f, -0.5f); // Смещаем хитбокс для сидения, чтобы не пересекался с полом
        }
        else
        {
            boxCollider.size = standingColliderSize; // Восстанавливаем обычный хитбокс
            boxCollider.offset = Vector2.zero; // Восстанавливаем смещение хитбокса в исходное состояние
            animator.SetTrigger("CrouchToIdle"); // Используем триггер для плавного перехода из сидячего состояния в стоячее
        }
    }

    void UpdateAnimation(float moveInput)
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput));  // Передаем скорость в Animator
        animator.SetBool("IsGrounded", isGrounded); // Обновляем состояние "на земле"
        animator.SetBool("IsCrouching", isCrouching); // Обновляем состояние "сидит"
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.ResetTrigger("JumpTrigger"); // Сбрасываем триггер прыжка при приземлении
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
