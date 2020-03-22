using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_GameManager : MonoBehaviour {

    public static CS_GameManager instance;
    public CS_MatchSettings matchSettings;
    public Color[] colors;
    const string PLAYER_ID_PREFIX = "Player ", BOT_ID_PREFIX = "Bot ";

    public delegate void OnPlayerKilledCallback(string player, Color playerColor, string source, Color sourceColor);
    public OnPlayerKilledCallback onPlayerKilledCallback;

    #region Player Tracking
    public static Dictionary<string, CS_Player> players = new Dictionary<string, CS_Player>();
    public static Dictionary<string, CS_Bot> bots = new Dictionary<string, CS_Bot>();

    public static void RegisterPlayer (string _netID, CS_Player _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        _player.transform.name = _playerID;

        players.Add(_playerID, _player);
    }

    public static void RegisterBot( string _netID, CS_Bot _bot)
    {
        string _botID = BOT_ID_PREFIX + _netID;
        _bot.transform.name = _botID;

        bots.Add(_botID, _bot);
    }

    public static void UnRegisterPlayer (string _playerID)
    {
        players.Remove(_playerID);
    }

    public static void UnRegisterBot (string _botID)
    {
        bots.Remove(_botID);
    }

    public static CS_Player GetPlayer (string _charID)
    {
        if (players.ContainsKey(_charID))
            return players[_charID];
        else return null;
    }

    public static CS_Bot GetBot (string _charID)
    {
        if (bots.ContainsKey(_charID))
            return bots[_charID];
        else return null;
    }

    //private void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(200, 200, 200, 500));

    //    GUILayout.BeginVertical();

    //    foreach (string _playerID in players.Keys) {
    //        GUILayout.Label(_playerID + " - " + players[_playerID].transform.name);
    //    }

    //    GUILayout.EndVertical();

    //    GUILayout.EndArea();
    //}
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public Transform GetClosestPlayer()
    {
        Transform tMin = null;
        Vector3 currentPos = transform.position;

        float minDist = Mathf.Infinity;

        foreach (KeyValuePair<string, CS_Player> t in players) {
            float dist = Vector3.Distance(t.Value.transform.position, currentPos);
            if (dist < minDist && !t.Value.isDead) {
                tMin = t.Value.transform;
                minDist = dist;
            }
        }
        return tMin;
    }
}
