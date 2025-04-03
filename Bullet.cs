using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float damage = 10f;
    [Tooltip("Через сколько секунд пуля уничтожится автоматически (если ни во что не врежется).")]
    public float lifeTime = 2f;
    [Tooltip("Префаб эффекта при попадании (частицы, вспышка и т.п.). Не обязателен.")]
    public GameObject impactEffect;

    void Start()
    {
        // Уничтожить пулю через lifeTime сек, если она ничего не заденет
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Если есть EnemyHealth – наносим урон
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        // Если есть эффект при попадании – заспавним
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Уничтожаем пулю при любом столкновении
        Destroy(gameObject);
    }
}
