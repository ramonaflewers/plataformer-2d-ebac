using UnityEngine;
using System;

public class HealthBase : MonoBehaviour
{
    public event Action OnDeath;
    public int startingHealth = 10;
    private int _currentHealth;
    private bool _isDead = false;
    public bool DestroyOnDeath = false;
    public float DelayToDestroy = 2f;

    [SerializeField] private FlashColor _flashColor;

    private void Awake()
    {
        init();
        if (_flashColor == null)
        {
            _flashColor = GetComponent<FlashColor>();
        }
    }

    private void init()
    {
        _currentHealth = startingHealth;
        _isDead = false;
    }

    public void Damage(int damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;

        if (_currentHealth > 0)
        {
            if (_flashColor != null)
            {
                _flashColor.Flash();
            }
        }
        else
        {
            kill();
        }
    }

    private void kill()
    {
        _isDead = true;
        if (DestroyOnDeath)
        {
            Destroy(gameObject, DelayToDestroy);
        }
        OnDeath?.Invoke();
    }
}
