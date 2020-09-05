using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEnergy : MonoBehaviour, IDamagable
{
    [SerializeField] private int maxEnergy = 10;

    private int _currentEnergy;

    public void Die()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TakeDamage(int amount)
    {
        _currentEnergy -= amount;
        UpdateEnergy();
        if (_currentEnergy <= 0)
            Die();
    }

    public void UpdateEnergy(int maxEnergy, int currentEnergy)
    {
        UIManager.instance.UpdateEnergy(maxEnergy, currentEnergy);
    }

    private void UpdateEnergy()
    {
        UIManager.instance.UpdateEnergy(maxEnergy, _currentEnergy);
    }
}
