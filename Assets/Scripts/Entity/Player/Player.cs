using UnityEngine;
using UnityEngine.Events;

public class Player : Entity 
{
    public UnityEvent onDie;

    protected override void Die()
    {
        onDie.Invoke();
        Destroy(gameObject);
    }
}