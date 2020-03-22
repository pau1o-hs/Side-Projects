using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_Username : MonoBehaviour {

    [SerializeField] InputField usernameText;
    [SerializeField] Text statusText;
    public Color positive, negative;
    
	// Use this for initialization
	void Start () {
        usernameText.text = PlayerPrefs.GetString("Username");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GetUsername(string _name)
    {
        if (_name != PlayerPrefs.GetString("Username")) {
            statusText.text = "Not applied";
            statusText.color = negative;
        }
        else
            statusText.color = Color.clear;
    }

    public void SetUsername ()
    {
        if (usernameText.text != "") {
            PlayerPrefs.SetString("Username", usernameText.text);
            PlayerPrefs.Save();
            statusText.text = "Username applied";
            statusText.color = positive;
        }
        else {
            BlankField();
        }
    }

    public void BlankField ()
    {
        statusText.text = "Blank Field";
        statusText.color = negative;
    }
}
