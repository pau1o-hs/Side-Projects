using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour {

    Camera m_Camera;

    void Update()
    {
        m_Camera = Camera.main;

        if (m_Camera != null) {
            transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
                m_Camera.transform.rotation * Vector3.up);
        }
    }
}