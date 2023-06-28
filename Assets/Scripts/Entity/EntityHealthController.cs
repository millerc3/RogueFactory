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

    public delegate void DamageAtPoint(int amount, Vector3? point);
    public DamageAtPoint OnDamageAtPoint;

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
        OnDamageAtPoint?.Invoke(damage, null);
    }

    public virtual void DamageAt(int damage, Vector3 position)
    {
        SetHealth(Mathf.Max(0, Health - damage));
        OnDamageAtPoint?.Invoke(damage, position);
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
