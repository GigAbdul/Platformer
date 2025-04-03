using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // Префаб пули, который нужно создать в Unity и назначить в инспекторе
    public GameObject bulletPrefab;
    // Точка, откуда будет появляться пуля (например, пустой GameObject, дочерний объект персонажа)
    public Transform firePoint;
    // Скорость движения пули
    public float bulletSpeed = 10f;
    // Интервал между выстрелами (в секундах)
    public float fireRate = 0.5f;
    // Время, когда можно будет сделать следующий выстрел
    private float nextFireTime = 0f;

    void Update()
    {
        // Проверяем нажатие кнопки "Fire1" (обычно левая кнопка мыши или Ctrl)
        if (Input.GetButton("Fire1") && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        // Создаем экземпляр пули в позиции firePoint с ее поворотом
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        // Получаем компонент Rigidbody2D пули и задаем ей скорость в направлении firePoint.right
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = firePoint.right * bulletSpeed;
        }
    }
}
