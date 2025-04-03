using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 direction;
    public float speed = 10f;

    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection;
        Destroy(gameObject, 2f); // Destroy bullet after 2 seconds to prevent memory leaks
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Implement what happens on collision, e.g., damage an enemy
        Destroy(gameObject); // Destroy bullet on collision
    }
}