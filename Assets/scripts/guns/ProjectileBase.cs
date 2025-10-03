using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public Vector3 direction;
    public float speed = 5f;
    public float destroyTime = 2f;
    public int damage = 5;

    private void Awake()
    {
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var health = other.GetComponent<HealthBase>();

        if (health != null)
        {
            health.Damage(damage);
            Destroy(gameObject);
        }
    }
}
