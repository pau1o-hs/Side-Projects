using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class CS_JoinGame : MonoBehaviour {

    [SerializeField] private Text status;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Transform roomListParent;

    List<GameObject> roomList = new List<GameObject>();
    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = NetworkManager.singleton;
        networkManager.StopMatchMaker();
        networkManager.StartMatchMaker();

        RefreshRoomList();
    }

    public void RefreshRoomList ()
    {
        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        status.text = "";

        if (matches == null) {
            status.text = "Couldn't get matches";
            return;
        }

        ClearRoomList();

        foreach (MatchInfoSnapshot match in matches) {
            GameObject _roomListItemGO = Instantiate(roomListItemPrefab, roomListParent);

            CS_RoomListItem _roomListItem = _roomListItemGO.GetComponent<CS_RoomListItem>();
            if (_roomListItem != null)
                _roomListItem.Setup(match, JoinRoom);

             roomList.Add(_roomListItemGO);
        }

        if (roomList.Count == 0){
            status.text = "No matches at moment";
        }
    }

    void ClearRoomList ()
    {
        for (int i = 0; i < roomList.Count; i++)
            Destroy(roomList[i]);

        roomList.Clear();
    }

    public void JoinRoom (MatchInfoSnapshot _match)
    {
        if (PlayerPrefs.GetString("Username") == "" || PlayerPrefs.GetString("Username") == null) {
            GameObject.FindObjectOfType<CS_Username>().BlankField();
            return;
        }

        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();
        status.text = "Joining...";
    }
}
