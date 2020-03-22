using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_PlayerNameplate : MonoBehaviour {

    [SerializeField] private CS_Player m_Player;
    [SerializeField] private Text usernameText;
    [SerializeField] private RectTransform healthBarFill;

	// Update is called once per frame
	void Update () {

        usernameText.text = m_Player.GetUsername();
        usernameText.color = m_Player.m_Color;
        healthBarFill.localScale = new Vector3(1, m_Player.GetHealthAmount() / m_Player.maxHealth, 1);
        healthBarFill.GetComponent<Image>().color = m_Player.m_Color;
    }
}
