using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;   // Префаб пули
    public float bulletSpeed = 10f;   // Скорость полёта пули
    public Transform firePoint;       // Точка выстрела

    private SpriteRenderer spriteRenderer; // Если нужно поворачивать спрайт персонажа

    void Start()
    {
        // Если скрипт на персонаже, и у него есть SpriteRenderer
        // Если нет — можно убрать эту строку
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            // Смотрим влево, стреляем влево
            FaceDirection(Vector2.left);
            Shoot(Vector2.left);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            // Смотрим вниз, стреляем вниз
            // Тут может понадобиться повернуть спрайт на 90 градусов
            FaceDirection(Vector2.down);
            Shoot(Vector2.down);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            // Смотрим вправо, стреляем вправо
            FaceDirection(Vector2.right);
            Shoot(Vector2.right);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            // Смотрим вверх, стреляем вверх
            FaceDirection(Vector2.up);
            Shoot(Vector2.up);
        }
    }

    void Shoot(Vector2 direction)
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Не заданы префаб пули или firePoint!");
            return;
        }

        // Создаём пулю
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // У пули должен быть Rigidbody2D, чтобы задать скорость
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction.normalized * bulletSpeed;
        }
    }

    // Если нужно «поворачивать» персонажа или его оружие:
    // Самый простой случай — для лево/право меняем flipX
    // а для вверх/вниз можно добавить дополнительную логику, если у вас есть отдельное оружие, которое крутится
    void FaceDirection(Vector2 direction)
    {
        // Простой вариант — если влево, flipX = true, если вправо, flipX = false
        // А вниз/вверх тут не трогаем, т.к. обычно flipX отвечает только за горизонтальное направление
        if (spriteRenderer != null)
        {
            if (direction == Vector2.left)
            {
                spriteRenderer.flipX = true;
            }
            else if (direction == Vector2.right)
            {
                spriteRenderer.flipX = false;
            }
        }

        // Если хотите обрабатывать спрайт или анимацию для вверх/вниз — придётся
        // делать отдельные повороты или анимационные состояния
    }
}
