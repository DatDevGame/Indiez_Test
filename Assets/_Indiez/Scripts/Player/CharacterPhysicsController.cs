using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterPhysicsController : MonoBehaviour
{
    [SerializeField] private float m_Gravity = -9.81f;
    [SerializeField] private float m_GroundCheckDistance = 0.3f;
    [SerializeField] private LayerMask m_GroundMask;
    [SerializeField] private float m_Drag = 3f;

    private CharacterController m_Controller;
    private Vector3 m_Velocity;
    private bool m_IsGrounded;

    private void Awake()
    {
        m_Controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!m_Controller.enabled)
            return;

        m_IsGrounded = Physics.CheckSphere(transform.position + Vector3.down * m_GroundCheckDistance, 0.2f, m_GroundMask);
        if (m_IsGrounded && m_Velocity.y < 0)
            m_Velocity.y = -2f;
        m_Velocity.y += m_Gravity * Time.deltaTime;
        m_Velocity -= m_Velocity * m_Drag * Time.deltaTime;
        m_Controller.Move(m_Velocity * Time.deltaTime);
    }

    public void AddForce(Vector3 force)
    {
        m_Velocity += force;
    }
}
