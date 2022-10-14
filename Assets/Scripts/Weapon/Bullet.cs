using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 15;
    private ParticleSystem m_particleFX;
    private List<ParticleCollisionEvent> m_colEvents = new List<ParticleCollisionEvent>();

    private void Start() 
    {
        m_particleFX = GetComponent<ParticleSystem>();
    }
    
    private void OnParticleCollision(GameObject other) 
    {   
        if(other.CompareTag("Enemy"))
        {
            int events = m_particleFX.GetCollisionEvents(other, m_colEvents);
            Enemy enemy = other.GetComponent<Enemy>();
            for(int i = 0; i < events; i++)
            {
                enemy.ApplyDamage(damage);
            }
        }
    }
}
