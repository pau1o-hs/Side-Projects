using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Darkness : MonoBehaviour {

    public float speed;
    Light m_Light;

	// Use this for initialization
	void Start () {

        m_Light = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {

        m_Light.range = Mathf.MoveTowards(m_Light.range, 0, speed * Time.deltaTime);
	}
}
