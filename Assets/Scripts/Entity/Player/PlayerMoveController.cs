using UnityEngine;

public class PlayerMoveController : MonoBehaviour 
{
    [SerializeField, Range(0f, 30f)] protected float m_maxSpeed = 4f;
    public float MaxSpeed
	{
		get { return m_maxSpeed; }
		set { m_maxSpeed = value; }
	}
    
    [SerializeField, Range(0f, 1f)] protected float m_moveSmooth = .3f;
    [SerializeField, Range(0f, 1f)] protected float m_rotationSmooth = .3f;
    
    [SerializeField] protected Transform m_root;
    public Transform root
    {
        get { return m_root; }
    }
    
    protected Vector2 m_input;
    protected Vector3 m_direction, m_desiredDirection, m_velocity;
    protected Rigidbody m_body;
    protected Animator m_animator;
    protected CharacterController m_characterController;
    protected PlayerWeaponController m_weaponController;

    protected bool overrideDirection = false;

    protected virtual void Awake() 
    {
		m_body = GetComponent<Rigidbody>(); 
        m_animator = GetComponentInChildren<Animator>();
        m_characterController = GetComponent<CharacterController>(); 
        m_weaponController = GetComponent<PlayerWeaponController>();
    }

    protected virtual void Update()
    {
        MoveAction();
    }

    protected virtual void MoveAction()
    {
        if(LevelManager.i.LevelEnded)
        {
            m_direction = Vector3.zero;
        }
        else
        {
            m_direction = new Vector3(InputManager.i.MoveInput.Horizontal, 0, InputManager.i.MoveInput.Vertical);
        }

        if(m_direction != Vector3.zero) 
        {
            m_animator.SetBool("isMove", true);
        } 
        else 
        {
            m_animator.SetBool("isMove", false);
        }

        m_desiredDirection = Vector3.SmoothDamp(m_desiredDirection, m_direction.normalized, ref m_velocity, m_moveSmooth);
        m_characterController.Move(m_desiredDirection * m_maxSpeed * Time.deltaTime);

        if(m_weaponController.IsShooting)
        {
            Quaternion targetRotation = Quaternion.LookRotation(m_weaponController.shootingDirection, Vector3.up);
            m_root.transform.rotation = Quaternion.Slerp(m_root.transform.rotation, targetRotation, m_rotationSmooth);
        }
        else 
        {
            if(m_direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(m_direction, Vector3.up);
                m_root.transform.rotation = Quaternion.Slerp(m_root.transform.rotation, targetRotation, m_rotationSmooth);
            }
        }
    }
}