using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    CircleCollider2D m_Collider;
    CharacterScript player;

    public bool oneHit = true;
    [HideInInspector] public bool active; 
    bool canActivate;

    void Start()
    {
        m_Collider = GetComponent<CircleCollider2D>();
        m_Collider.enabled = true;
    }

    void Update()
    {

        if (canActivate && Input.GetButtonDown("P" + player.player + "_Interact") && !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Anim_Button")) {

            StartCoroutine(Timerbtn());
            GetComponent<Animator>().SetTrigger("Activate");
            player.anim.SetTrigger("Interact");
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
            active = !active;
            CS_GameManager.instance.UpdateEvent();
            active = false;
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

    IEnumerator Timerbtn()
    {
       m_Collider.enabled = true;
       yield return new WaitForSeconds(3);
       m_Collider.enabled = true;
    }
}
