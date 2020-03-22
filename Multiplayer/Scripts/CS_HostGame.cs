using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CS_HostGame : MonoBehaviour {

    [SerializeField] private uint roomSize = 6;
    private string roomName;
    private NetworkManager networkManager;

    public InputField roomNameField;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        networkManager.StopMatchMaker();
        networkManager.StartMatchMaker();

        if (PlayerPrefs.GetString("Username") != null)
            roomNameField.text = PlayerPrefs.GetString("Username") + "'s match"; 
    }

    public void SetRoomName (string _name)
    {
        roomName = _name;
    }

    public void CreateRoom ()
    {
        if (PlayerPrefs.GetString("Username") == "" || PlayerPrefs.GetString("Username") == null) {
            GameObject.FindObjectOfType<CS_Username>().BlankField();
        }

        if (roomName != "" && roomName != null) {

            Debug.Log("Creating Room:" + roomName + " for " + roomSize + " players");
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
    }
}
