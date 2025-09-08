using UnityEngine;

public class HealthBase : MonoBehaviour
{
    public int startingHealth = 10;
    private int _currentHealth;
    private bool _isDead = false;
    public bool DestroyOnDeath = false;

    public float DelayToDestroy = 2f;

    private void Awake()
    {
        init();
    }

    private void init(){
        _currentHealth = startingHealth;
        _isDead = false;;
    }

    public void Damage(int damage){
        if (_isDead) return;

        _currentHealth -= damage;

        if (_currentHealth <= 0){
            kill();
        }
    }

    private void kill(){
        _isDead = true;
        if (DestroyOnDeath){
            Destroy(gameObject);
        }
    }
}
