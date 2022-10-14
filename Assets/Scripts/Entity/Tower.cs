using UnityEngine;
using UnityEngine.Events;

public class Tower : Entity
{
    public UnityEvent onDestroyed;

    protected override void Die()
    {
        onDestroyed.Invoke();
    }
}
