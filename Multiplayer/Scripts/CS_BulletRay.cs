using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_BulletRay : MonoBehaviour {

    LineRenderer line;
	// Use this for initialization
	void Start () {

        line = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

        line.SetPosition(0, Vector3.MoveTowards(line.GetPosition(0), line.GetPosition(1), 50 * Time.deltaTime));
	}
}
