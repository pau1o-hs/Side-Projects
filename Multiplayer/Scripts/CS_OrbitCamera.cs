using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_OrbitCamera : MonoBehaviour {

    public Transform target;
    public float followSpeed, sensitivity, targetDistance, aimSensitivity, minAngle, maxAngle, defaultFOV = 60, aimFOV;
    float currentSensitivity, currentFOV, currentDistance;

    Transform holder, pivot;
    [HideInInspector] public Animator m_Animator;
    [HideInInspector] public Camera m_Cam;
    float angle, recoilMultiplier;
    
    CS_PlayerSetup m_Setup;

	// Use this for initialization
	void Start () {

        target = transform.parent;
        m_Setup = target.GetComponent<CS_PlayerSetup>();
        m_Cam = m_Setup.m_Camera;
        pivot = transform.GetChild(0);
        holder = pivot.GetChild(0);
        transform.parent = null;

        if (PlayerPrefs.GetFloat("Mouse Sensitivity") == 0)
            PlayerPrefs.SetFloat("Mouse Sensitivity", 1);

        m_Animator = GetComponent<Animator>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate () {

        if (currentSensitivity != PlayerPrefs.GetFloat("Mouse Sensitivity"))
            currentSensitivity = PlayerPrefs.GetFloat("Mouse Sensitivity");

        if (target == null)
            Destroy(gameObject);

        //LOCK CURSOR
        if (CS_Pause.isOn) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //MOVE AND ROTATE
        float h = Input.GetAxis("Mouse X") * currentSensitivity;
        float v = Input.GetAxis("Mouse Y") * -currentSensitivity;

        angle += v;
        angle = Mathf.Clamp(angle, -minAngle, maxAngle);

        transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
        transform.Rotate(0, h, 0);
        pivot.localRotation = Quaternion.Euler(angle, 0, 0);

        //AIM
        if (Input.GetMouseButtonDown(1)) {
            currentSensitivity = aimSensitivity;
            m_Animator.SetTrigger("InAim");
            m_Setup.m_Canvas.showAim = true;
        }

        if (Input.GetMouseButtonUp(1)) {
            currentSensitivity = sensitivity;
            m_Animator.SetTrigger("OutAim");
            m_Setup.m_Canvas.showAim = false;
        }

        //CAMERA COLLISION
        RaycastHit hit;
        if (Physics.Raycast(target.position, holder.position - target.position, out hit, targetDistance)) {

            currentDistance = Mathf.Clamp(-hit.distance, -1, -targetDistance);
        }
        else currentDistance = -targetDistance;

        holder.localPosition = Vector3.MoveTowards(holder.localPosition, Vector3.forward * currentDistance, 5 * Time.deltaTime);

        if (recoilMultiplier > 0)
            recoilMultiplier -= 15 * Time.deltaTime;
    }

    public void Recoil(float recoil)
    {
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Anim_CamAim")) recoilMultiplier += 1.25f;
        else recoilMultiplier += 1.5f;

        transform.Rotate(0, recoilMultiplier * Random.Range(-recoil, recoil), 0);
        angle -= recoilMultiplier * recoil;
    }
}
