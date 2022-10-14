using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRangeIndicator : MonoBehaviour
{
    [SerializeField] private Weapon m_weapon;
    [SerializeField] private LayerMask m_whatIsEnvironment;

    private Canvas m_canvas;
    private RectTransform m_rectTransform;

    private bool m_isCalculateRange;

    private void Awake() 
    {
        m_canvas = GetComponent<Canvas>();
        m_rectTransform = GetComponent<RectTransform>();
    }

    private void Start() 
    {
        SetActive(false);
        SetRange(m_weapon.range); 

        InputManager.i.AttackInput.onPointerDown.AddListener(OnPointerDownAttackInput);
        InputManager.i.AttackInput.onDrag.AddListener(OnDragAttackInput); 
        InputManager.i.AttackInput.onPointerUp.AddListener(OnPointerUpAttackInput);
    }

    private void LateUpdate() 
    {
        if(!m_isCalculateRange) return;
        
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, m_weapon.range, m_whatIsEnvironment))
        {
            SetRange(Vector3.Distance(transform.position, hit.point));
        }
        else
        {
            SetRange(m_weapon.range);
        }
    }

    private void OnPointerDownAttackInput()
    {
        SetActive(true);
        m_isCalculateRange = true;
    }

    private void OnDragAttackInput(Vector2 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y), Vector3.up);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

    }

    private void OnPointerUpAttackInput()
    {
        SetActive(false);
        m_isCalculateRange = false;
    }

    public void SetActive(bool val)
    {
        m_canvas.enabled = val;
    }

    public void SetRange(float range)
    {
        m_rectTransform.sizeDelta = new Vector2(m_rectTransform.sizeDelta.x, range);
    }
}
