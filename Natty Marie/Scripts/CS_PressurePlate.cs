using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_PressurePlate : MonoBehaviour {

    AudioSource audSource;
    [HideInInspector] public bool active;

    public bool hold;

    // Use this for initialization
    void Start () {

        audSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

        GetComponent<Animator>().SetBool("Activated", active);

        Debug.DrawRay(transform.position + Vector3.up * .2f + Vector3.right * -.8f, Vector3.right * 1.5f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * .2f + Vector3.right * -.8f, Vector3.right, 1.5f);
        if (hit.collider != null && !active) {

            active = true;
            CS_GameManager.instance.UpdateEvent();

            if (audSource.isPlaying)
                audSource.Stop();

            audSource.PlayOneShot(audSource.clip);
        }

        if (hit.collider == null && hold && active) {

            active = false;
            CS_GameManager.instance.UpdateEvent();

            if (audSource.isPlaying)
                audSource.Stop();

            audSource.PlayOneShot(audSource.clip);
        }
    }
}
