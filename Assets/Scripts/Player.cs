using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float m_speed = 5.0f;
    [SerializeField] float m_angularSpeed = 50.0f;

    public Vector2 position { get => new Vector2(transform.position.x, transform.position.z); }

    private void Update()
    {
        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * m_angularSpeed * Time.deltaTime);
        
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.Z))
        {
            direction += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += -transform.forward;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            direction += -transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += transform.right;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            direction += Vector3.up;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            direction += Vector3.down;
        }

        direction.Normalize();

        transform.position += direction * m_speed * Time.deltaTime;
    }
}
