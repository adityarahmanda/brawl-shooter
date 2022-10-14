using System.Collections;
using UnityEngine;

public enum WeaponType
{
    SingleShot,
    RapidShot
}

public class Weapon : MonoBehaviour 
{    
    public WeaponType type;

    [Range(0, 30f)] public float range = 10f;

    [SerializeField, Range(0, 10)] private int m_magazineSize = 3;
    public int magazineSize
    {
        get { return m_magazineSize; }
        private set { m_magazineSize = value; }
    }

    [SerializeField, Range(0, 10f)] private float m_bulletsCooldownTime = .3f;
    public float bulletsCooldownTime
    {
        get { return m_bulletsCooldownTime; }
        private set { m_bulletsCooldownTime = value; }
    }

    [SerializeField, Range(0, 1f)] private float m_timeBetweenShoot = .3f;
    public float timeBetweenShoot
    {
        get { return m_timeBetweenShoot; }
        private set { m_timeBetweenShoot = value; }
    }

    [HideInInspector] public float bulletsLeft;
}