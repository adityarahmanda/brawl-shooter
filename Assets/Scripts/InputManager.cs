using UnityEngine;

public class InputManager : MonoBehaviour 
{
    public static InputManager i;

    private void Awake() 
    {
        if(i == null)
        {
            i = this;
        }
        else
        {
            Destroy(this);
        }
    }

    [SerializeField] private Joystick m_moveInput;
    public Joystick MoveInput
    {
        get { return m_moveInput; }
    }

    [SerializeField] private Joystick m_attackInput;
    public Joystick AttackInput
    {
        get { return m_attackInput; }
    }
}