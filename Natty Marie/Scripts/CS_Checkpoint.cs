using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Checkpoint : MonoBehaviour {

    public int index;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player") {

            PlayerPrefs.SetInt("Checkpoint", index);
            PlayerPrefs.Save();
            GetComponent<Light>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == "Player") {

            GetComponent<Light>().enabled = false;
        }
    }
}
