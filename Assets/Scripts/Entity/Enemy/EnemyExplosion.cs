using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    public int damage = 30;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Tower"))
        {
            Entity entity = other.gameObject.GetComponent<Entity>();
            entity.ApplyDamage(damage);
        }
    }
}
