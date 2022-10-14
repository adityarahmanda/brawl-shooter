using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    protected Transform m_mainCamera;
    protected float m_currentValue, m_maxValue;

    [SerializeField] protected bool isWorldSpace;
    [SerializeField] protected Image m_progressImage;

    protected virtual void Start() 
    {
        m_mainCamera = Camera.main.transform;
    }

    protected virtual void Update() 
    {
        if(isWorldSpace)
        {
            LookAtCamera();
        }
    }

    protected virtual void LookAtCamera()
    {
        transform.LookAt(transform.position + m_mainCamera.rotation * Vector3.back, m_mainCamera.rotation * Vector3.down);
    }

    public virtual void SetProgress(float currentValue, float maxValue)
    {
        m_progressImage.fillAmount = currentValue / maxValue;
    }
}
