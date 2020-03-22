using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]

public class CS_Controller : NetworkBehaviour {

    [HideInInspector] public CS_OrbitCamera m_OrbitCam;
    [HideInInspector] public bool landEffect;

    bool onGround = true;
    public float checkGroundDistance, cgdGrounded, cgdJumped;
    public float moveSpeed, turnSpeed;
    public float gravityMultiplier, jumpForce, airControl, airSpeed;
    Rigidbody rb;
    Vector3 movement;

    // Use this for initialization
    void Start () {

        rb = GetComponent<Rigidbody>(); 

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate () {

        if (CS_Pause.isOn) {
            rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(0, rb.velocity.y, 0), 1.5f * Time.deltaTime);
            return;
        }

        //INPUT
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool jumpInput = Input.GetKeyDown(KeyCode.Space);

        CheckGround();

        if (onGround) {
            Grounded(h, v);

            if (jumpInput)
                Jump(h, v);
        }
        else
            Airborne(h, v);
    }

    void CheckGround ()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, checkGroundDistance)) {

            if (!onGround)
                m_OrbitCam.m_Animator.SetTrigger("LandEffect");
            onGround = true;
        }
        else {
            onGround = false;
        }

        //ROTATE
        rb.rotation = Quaternion.Lerp(rb.rotation, m_OrbitCam.transform.rotation, turnSpeed * Time.deltaTime);
    }

    void Grounded (float h, float v)
    {
        checkGroundDistance = cgdGrounded;

        //CALCULATE DIRECTION
        movement = (h * m_OrbitCam.transform.right + v * m_OrbitCam.transform.forward);
        if (movement.magnitude > 1f)
            movement.Normalize();

        //MOVE
        if (h != 0 || v != 0)
            rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);
        else
            rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(0, rb.velocity.y, 0), 1.5f * Time.deltaTime);
    }

    void Airborne (float h, float v)
    {
        Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
        rb.AddForce(extraGravityForce);

        Vector3 airMove = movement = (h * m_OrbitCam.transform.right + v * m_OrbitCam.transform.forward);
        if (airMove.magnitude > 1f)
            airMove.Normalize();

        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(airMove.x * airSpeed, rb.velocity.y, airMove.z * airSpeed), airControl * Time.deltaTime);
    }

    void Jump (float h, float v)
    {
        checkGroundDistance = cgdJumped;
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }
}
