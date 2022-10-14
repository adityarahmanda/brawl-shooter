using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField, Range(5f, 15f)] private float m_spawnRate = 10f;
    [SerializeField] private Entity m_entityPrefab;

    private float m_spawnTimer;

    private void Start() 
    {
        m_spawnTimer = Random.Range(5f, m_spawnRate);
    }

    private void Update() 
    {
        if(LevelManager.i.LevelEnded) return;
        
        if(m_spawnTimer > 0)
        {
            m_spawnTimer -= Time.deltaTime;
        }
        else
        {
            Instantiate(m_entityPrefab, transform.position, transform.rotation);
            m_spawnTimer = Random.Range(5f, m_spawnRate);
        }
    }
}
