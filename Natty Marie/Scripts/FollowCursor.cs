using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);

        if (!CS_GameManager.isPaused) {
            if (Cursor.visible) {
                Cursor.visible = false;
                //GetComponent<TrailRenderer>().enabled = true;
            }
        }
        else if (!Cursor.visible) {
            Cursor.visible = true;
            GetComponent<TrailRenderer>().enabled = false;
        }
    }
}
