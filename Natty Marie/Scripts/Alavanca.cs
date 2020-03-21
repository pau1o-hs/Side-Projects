using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alavanca : MonoBehaviour {

    [HideInInspector] public bool active;
    CircleCollider2D m_Collider;

    bool canActivate;
    CharacterScript player;
    AudioSource audSource;

    // Use this for initialization
    void Start () {

        m_Collider = GetComponent<CircleCollider2D>();
        m_Collider.enabled = true;

        audSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {

        if (canActivate && Input.GetButtonDown("P" + player.player + "_Interact") && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= .95f)
        {
            active = !active;
            CS_GameManager.instance.UpdateEvent();
            GetComponent<Animator>().SetTrigger("Activate");
            player.anim.SetTrigger("Lever");
            audSource.PlayOneShot(audSource.clip);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            canActivate = true;
            player = other.GetComponent<CharacterScript>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            canActivate = false;
            player = null;
        }
    }
}
