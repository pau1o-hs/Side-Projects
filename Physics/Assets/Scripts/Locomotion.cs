using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    Transform m_Camera;
    Rigidbody2D rb;
    LineRenderer drawForce;

    public float maxVelocity;
    public float acceleration;

    float h;
    float v;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main.transform;
        rb = GetComponent<Rigidbody2D>();
        //trail = GetComponentInChildren<TrailRenderer>();
        drawForce = GetComponentInChildren<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector2 movement = (h * m_Camera.right + v * m_Camera.up);
        movement.Normalize();

        rb.AddForce(movement * acceleration * Time.deltaTime, ForceMode2D.Force);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);

        drawForce.SetPosition(1, -rb.velocity * 10);

        if (movement.magnitude >= 1)
            rb.rotation = Vector2.SignedAngle(Vector2.up, movement);

    }
}

