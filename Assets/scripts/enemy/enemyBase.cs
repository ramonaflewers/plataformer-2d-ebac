using UnityEngine;
using System.Collections;

public class enemyBase : MonoBehaviour
{
    public int damage = 10;
    public float speed = 3f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;

    public Animator animator;
    public string triggerAttack = "attack";
    public string triggerDeath = "death";
    public string triggerWalk = "walk";

    public HealthBase healthBase;
    public Transform visual;
    public Rigidbody2D rb; // ← novo: para parar movimento com física (se usado)

    private Transform player;
    private bool canAttack = true;
    private bool isDead = false;
    private bool isAttacking = false;

    private void Awake()
    {
        if (healthBase != null)
            healthBase.OnDeath += OnEnemyDeath;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Virar para o player
        if (visual != null)
        {
            float direction = player.position.x - transform.position.x;
            Vector3 scale = visual.localScale;
            scale.x = Mathf.Abs(scale.x) * (direction >= 0 ? 1 : -1);
            visual.localScale = scale;
        }

        if (distance > attackRange)
        {
            if (!isAttacking)
            {
                Vector2 moveDir = (player.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

                animator.ResetTrigger(triggerAttack);
                animator.SetTrigger(triggerWalk);
            }
        }
        else
        {
            animator.ResetTrigger(triggerWalk);

            if (canAttack && !isAttacking)
            {
                StartCoroutine(PerformAttack());
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        canAttack = false;

        animator.SetTrigger(triggerAttack);

        yield return new WaitForSeconds(0.7f); // tempo da animação

        if (player != null && !isDead)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= attackRange)
            {
                HealthBase playerHealth = player.GetComponent<HealthBase>();
                if (playerHealth != null)
                    playerHealth.Damage(damage);
            }
        }

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        isAttacking = false;
    }

    private void OnEnemyDeath()
    {
        if (isDead) return;

        isDead = true;

        StopAllCoroutines();

        animator.ResetTrigger(triggerAttack);
        animator.ResetTrigger(triggerWalk);
        animator.SetTrigger(triggerDeath);

        // Parar fisicamente
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        StartCoroutine(DestroyAfterDelay(1f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public void Damage(int amount)
    {
        if (isDead) return;

        if (healthBase != null)
            healthBase.Damage(amount);
    }
}
