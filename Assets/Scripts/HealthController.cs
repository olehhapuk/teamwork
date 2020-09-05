using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth;
    
    private float _currentHealth;

    private void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
