using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

    [Header("Weapon Settings")]
    // Точка, откуда производится выстрел (например, дуло оружия). Если не задана, используется позиция объекта.
    public Transform weaponTransform;
    
    [Header("Animation & Sprite")]
    // Ссылки на компоненты для анимации стрельбы и управления направлением взгляда
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    void Update()
    {
        // Выстрел вверх
        if (Input.GetKeyDown(KeyCode.I))
        {
            Shoot(Vector2.up);
        }
        // Выстрел влево и поворот влево
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (spriteRenderer != null)
                spriteRenderer.flipX = true;
            Shoot(Vector2.left);
        }
        // Выстрел вниз
        if (Input.GetKeyDown(KeyCode.K))
        {
            Shoot(Vector2.down);
        }
        // Выстрел вправо и поворот вправо
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (spriteRenderer != null)
                spriteRenderer.flipX = false;
            Shoot(Vector2.right);
        }
    }

    void Shoot(Vector2 direction)
    {
        // Запуск соответствующей анимации стрельбы
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
        }

        // Определяем позицию появления пули
        Vector3 spawnPosition = (weaponTransform != null) ? weaponTransform.position : transform.position;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // Инициализация пули с заданным направлением и скоростью
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.Initialize(direction, bulletSpeed);
        }
    }
}
