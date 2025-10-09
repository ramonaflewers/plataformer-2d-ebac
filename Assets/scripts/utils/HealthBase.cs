using UnityEngine;
using System;

public class HealthBase : MonoBehaviour
{
    public event Action OnDeath;
    public int startingHealth = 10;
    private int _currentHealth;
    private bool _isDead = false;
    public bool DestroyOnDeath = true;
    public float DelayToDestroy = 0f;

    [SerializeField] private FlashColor _flashColor;

    private void Awake()
    {
        _currentHealth = startingHealth;
        _isDead = false;

        if (_flashColor == null)
            _flashColor = GetComponent<FlashColor>();
    }

    public void Damage(int damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;

        if (_flashColor != null)
            _flashColor.Flash();

        if (_currentHealth <= 0)
            Kill();
    }

    public void Kill()
    {
        if (_isDead) return;

        _isDead = true;
        OnDeath?.Invoke();

        if (DestroyOnDeath)
            Destroy(gameObject, DelayToDestroy);
    }
}
