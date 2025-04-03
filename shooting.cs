using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    
    [Header("Weapon Settings")]
    // Точка, откуда производится выстрел (например, дуло оружия).
    public Transform weaponTransform;
    
    [Header("References")]
    // Если хотите, чтобы анимации стрельбы воспроизводились, назначьте ссылку на Animator и SpriteRenderer.
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    void Update()
    {
        // Клавиша J для выстрела
        if (Input.GetKeyDown(KeyCode.J))
        {
            Vector2 shootDirection = DetermineShootDirection();
            Shoot(shootDirection);
        }
    }

    // Определяет направление выстрела по нажатию клавиш.
    // I – вверх, K – вниз, L – вправо, U – влево.
    // Если никаких клавиш не нажато, используется направление из weaponTransform или направление, куда смотрит персонаж.
    Vector2 DetermineShootDirection()
    {
        Vector2 direction = Vector2.zero;

        if (Input.GetKey(KeyCode.I))
            direction += Vector2.up;
        if (Input.GetKey(KeyCode.K))
            direction += Vector2.down;
        if (Input.GetKey(KeyCode.L))
            direction += Vector2.right;
        if (Input.GetKey(KeyCode.U))
            direction += Vector2.left;

        // Если направление не задано, берём его из weaponTransform или из направления взгляда персонажа
        if (direction == Vector2.zero)
        {
            if (weaponTransform != null)
            {
                direction = weaponTransform.right;
            }
            else if (spriteRenderer != null)
            {
                direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
            }
            else
            {
                direction = Vector2.right; // Значение по умолчанию
            }
        }

        return direction.normalized;
    }

    void Shoot(Vector2 direction)
    {
        if (bulletPrefab != null)
        {
            // Выбираем позицию выстрела: если weaponTransform задан – используем её, иначе позицию объекта
            Vector3 spawnPosition = (weaponTransform != null) ? weaponTransform.position : transform.position;
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

            // Инициализируем пулю
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.Initialize(direction, bulletSpeed);
            }

            // Запуск анимаций стрельбы (если назначены)
            if (animator != null)
            {
                if (direction == Vector2.up)
                    animator.SetTrigger("ShootUp");
                else if (direction == Vector2.down)
                    animator.SetTrigger("ShootDown");
                else if (direction == Vector2.left)
                    animator.SetTrigger("ShootLeft");
                else if (direction == Vector2.right)
                    animator.SetTrigger("ShootRight");
                else
                    animator.SetTrigger("Shoot");
            }
        }
    }
}
