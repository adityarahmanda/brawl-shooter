using UnityEngine;

public class HealthBar : ProgressBar 
{
    [SerializeField] private Entity m_entity;

    protected override void Update() 
    {
        base.Update();
        SetProgress(m_entity.health, m_entity.maxHealth);
    }
}