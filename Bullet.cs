using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [Tooltip("Сколько урона наносит пуля.")]
    public float damage = 10f;
    
    [Tooltip("Время существования пули, после чего она уничтожится автоматически.")]
    public float lifeTime = 2f;

    [Tooltip("Префаб эффекта при попадании (частицы, вспышка и т.д.). Не обязателен.")]
    public GameObject impactEffect;

    void Start()
    {
        // Уничтожить пулю через lifeTime секунд,
        // если вдруг она ни во что не врежется
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Попробуем получить компонент здоровья (или врага)
        //    Здесь предполагается, что у врага есть скрипт EnemyHealth
        //    с методом TakeDamage(float damageAmount).
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        
        // Если у объекта есть такой скрипт — наносим урон
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        // 2. Если есть эффект попадания (частицы/вспышка) — создаём его
        //    на месте столкновения. 
        //    Можно также использовать collision.contacts[0].point
        //    чтобы точнее разместить эффект.
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // 3. Уничтожаем пулю
        Destroy(gameObject);
    }
}
