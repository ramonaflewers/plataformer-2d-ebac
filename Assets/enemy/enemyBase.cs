using UnityEngine;

public class enemyBase : MonoBehaviour
{
    public int damage=10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var health = collision.gameObject.GetComponent<HealthBase>();

        if (health != null)
        {
            health.Damage(damage);
        }
    }
}
