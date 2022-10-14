using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField, Range(0, 100f)] protected float m_health = 100f;
    public float health
    {
        get { return m_health; }
        private set { m_health = value; }
    }

    protected float m_maxHealth = 100f;
    public float maxHealth
    {
        get { return m_maxHealth; }
        private set { m_maxHealth = value; }
    }

    public virtual void ApplyDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            health = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}