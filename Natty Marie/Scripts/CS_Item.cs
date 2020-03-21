using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_Item : MonoBehaviour {

    GameObject player;

    [HideInInspector] public bool active;
    bool canPick, disable;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (disable && transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= .99f) {
            transform.GetChild(0).gameObject.SetActive(false);
            disable = false;
        }

        if (canPick && Input.GetButtonDown("P" + player.GetComponent<CharacterScript>().player + "_Interact")) {

            player.GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
            active = true;
            gameObject.SetActive(false);
            CS_GameManager.instance.UpdateEvent();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            transform.GetChild(0).gameObject.SetActive(true);
            player = other.gameObject;
            canPick = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            disable = true;
            player = null;
            canPick = false;
        }
    }
}
