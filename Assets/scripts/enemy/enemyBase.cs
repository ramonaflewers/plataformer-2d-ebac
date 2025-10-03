using UnityEngine;

public class enemyBase : MonoBehaviour
{
    public int damage = 10;
    public Animator animator;
    public string triggerAttack = "attack";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var health = collision.gameObject.GetComponent<HealthBase>();

        if (health != null)
        {
            health.Damage(damage);
            PlayAttackAnimation();
        }
    }

    private void PlayAttackAnimation()
    {
        animator.SetTrigger(triggerAttack);
    }
}
