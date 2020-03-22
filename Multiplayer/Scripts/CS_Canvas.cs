using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_Canvas : MonoBehaviour {

    public Image _crosshair, _killTarget, _hitTarget;
    private CS_Player m_Player;
    private CS_PlayerSetup m_Setup;

    [SerializeField] RectTransform healthBarFill;
    [SerializeField] GameObject _pause;
    [SerializeField] Image _panel;

    public bool showAim = false;

    // Use this for initialization
    void Start () {

        transform.SetParent(null);
        CS_Pause.isOn = false;

        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
	}
	
    public void SetPlayer(CS_Player _player)
    {
        m_Player = _player;
    }

	// Update is called once per frame
	void Update () {

        if (healthBarFill.GetComponent<Image>().color != m_Player.m_Color)
            healthBarFill.GetComponent<Image>().color = m_Player.m_Color;

        SetHealthAmount(m_Player.maxHealth, m_Player.GetHealthAmount());

        if (Input.GetKeyDown(KeyCode.Escape)) {

            TogglePause();
        }

        if (_killTarget.color != Color.clear)
            _killTarget.color = Color.Lerp(_killTarget.color, Color.clear, 3 * Time.deltaTime);

        if (_hitTarget.color != Color.clear)
            _hitTarget.color = Color.Lerp(_hitTarget.color, Color.clear, 2 * Time.deltaTime);

        if (_panel.color != Color.clear)
            _panel.color = Color.Lerp(_panel.color, Color.clear, 2 * Time.deltaTime);

        if (showAim)
             _crosshair.color = Color.Lerp(_crosshair.color, Color.white, 6 * Time.deltaTime);
        else if (_crosshair.color != Color.clear) _crosshair.color = Color.Lerp(_crosshair.color, Color.clear, 6 * Time.deltaTime);
    }

    public void TogglePause ()
    {
        _pause.SetActive(!_pause.activeSelf);
        CS_Pause.isOn = _pause.activeSelf;
    }

    void SetHealthAmount (float max, float _amount)
    {
        healthBarFill.localScale = new Vector3(1, _amount / max, 1);
    }

    public void KillEffect()
    {
        _killTarget.color = Color.white;
    }

    public void HitEffect ()
    {
        _hitTarget.color = Color.white;
    }

    public void HittedEffect()
    {
        _panel.color = Color.red;
    }
}
