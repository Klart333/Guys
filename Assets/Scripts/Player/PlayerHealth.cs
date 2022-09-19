using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int _maxHealth;

    private UIBarFiller _bar;
    private Player _player;

    private int _currentHealth;

    public int MaxHealth { get => _maxHealth; }
    public int CurrentHealth { get => _currentHealth; }

    private void Awake()
    {
        _currentHealth = _maxHealth;

        _bar = GetComponentInChildren<UIBarFiller>();
        _player = GetComponent<Player>();
    }

    public bool TakeDamage(int amount)
    {
        _currentHealth -= amount;

        float target = (float)_currentHealth / (float)_maxHealth;
        _bar.SetTargetFillAmount(target);

        if (_currentHealth <= 0)
        {
            Death();

            return true;
        }

        return false;
    }

    private void Death()
    {
        _player.Die();
        Destroy(gameObject, 0.1f);
    }
}
