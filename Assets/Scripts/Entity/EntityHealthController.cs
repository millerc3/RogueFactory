using UnityEngine;

public class EntityHealthController : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public int Health { get; private set; }
    [field: SerializeField] public int MaxHealth { get; private set; }
    public int PreviousHealth { get; private set; }

    public delegate void HealthChanged();
    public HealthChanged OnHealthChanged;

    public delegate void EntityDied();
    public EntityDied OnEntityDied;

    protected virtual void Awake()
    {
        SetHealth(MaxHealth);
    }

    public virtual void SetHealth(int health)
    {
        PreviousHealth = Health;
        Health = health;

        if (health <= 0)
        {
            Die();
        }

        OnHealthChanged?.Invoke();
    }

    public virtual void Damage(int damage)
    {
        SetHealth(Mathf.Max(0, Health - damage));
    }

    public virtual void Heal(int amount)
    {
        SetHealth(Mathf.Min(MaxHealth, Health + amount));
    }

    public virtual void Die()
    {
        OnEntityDied?.Invoke();
        Destroy(gameObject);
    }
}
