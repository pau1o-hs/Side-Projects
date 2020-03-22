using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CS_Player))]
[RequireComponent(typeof(CS_Controller))]
public class CS_PlayerSetup : NetworkBehaviour {

    CS_Player m_Player;
    public CS_OrbitCamera m_OrbitCam;
    public CS_Canvas m_Canvas;
    public Camera m_Camera;

    [SerializeField] Behaviour[] componentsToDisable;
    [SerializeField] GameObject[] dontDraw;
    Camera mainCamera;

    string username;
    Color m_Color;

    // Use this for initialization
    void Start()
    {
        m_Player = GetComponent<CS_Player>();
        GetComponent<CS_Controller>().m_OrbitCam = m_OrbitCam;
        if (!isLocalPlayer) {

            //DISABLE COMPONENTS
            for (int i = 0; i < componentsToDisable.Length; i++)
                componentsToDisable[i].enabled = false;

            //REMOTE PLAYER
            gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
        }
        else {
            mainCamera = Camera.main;
            AudioListener.volume = .5f;

            if (mainCamera != null)
                mainCamera.gameObject.SetActive(false);

            username = PlayerPrefs.GetString("Username");
            m_Color = CS_GameManager.instance.colors[Random.Range(0, CS_GameManager.instance.colors.Length)];
            //DONT DRAW
            for (int i = 0; i < dontDraw.Length; i++)
                dontDraw[i].layer = LayerMask.NameToLayer("DontDraw");

            m_Canvas.SetPlayer(m_Player);
            m_Player.m_Canvas = m_Canvas;
        }

        if (hasAuthority)
            CmdSendUsername(username, m_Color);

        m_Player.Setup();
    }

    [Command]
    void CmdSendUsername(string _name, Color _color)
    {
        m_Player.RpcGetPlayerInfo(_name, _color);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        CS_Player _player = GetComponent<CS_Player>();
        string _netId = GetComponent<NetworkIdentity>().netId.ToString();

        CS_GameManager.RegisterPlayer(_netId, _player);
    }

    private void OnDisable()
    {
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);

        if (isLocalPlayer) {
            if (m_OrbitCam != null)
                Destroy(m_OrbitCam.gameObject);
            if (m_Canvas != null)
                Destroy(m_Canvas.gameObject);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        CS_GameManager.UnRegisterPlayer(transform.name);
    }
}
