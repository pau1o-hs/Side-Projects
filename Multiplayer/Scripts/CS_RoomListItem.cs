using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class CS_RoomListItem : MonoBehaviour {

    public delegate void JoinRoomDelegate(MatchInfoSnapshot _match);
    private JoinRoomDelegate joinRoomCallback;

    [SerializeField] private Text roomName;
    [SerializeField] private Text connectedPlayers;

    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot _match, JoinRoomDelegate _joinRoomCallback)
    {
        match = _match;
        joinRoomCallback = _joinRoomCallback;

        roomName.text = match.name;
        connectedPlayers.text = "[ " + match.currentSize + " / " + match.maxSize + " ]";
    }

    public void JoinRoom ()
    {
        joinRoomCallback.Invoke(match);
    }
}
