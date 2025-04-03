using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Enemy took damage. Current HP = " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Здесь можно проиграть анимацию смерти, заспавнить эффект, дать очки игроку и т.п.
        // А затем уничтожить врага (или отключить его).
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}
