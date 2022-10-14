using UnityEngine;
using UnityEngine.UI;

public class BulletBar : ProgressBar 
{
    [SerializeField] private RawImage m_progressBorderTile;
    [SerializeField] private Weapon m_weapon;

    protected override void Start() 
    {
        base.Start();
        var uvRect = m_progressBorderTile.uvRect;
        m_progressBorderTile.uvRect = new Rect(uvRect.x, uvRect.y, m_weapon.magazineSize, uvRect.height);
    }

    protected override void Update() 
    {
        base.Update();
        SetProgress(m_weapon.bulletsLeft, m_weapon.magazineSize);
    }
}