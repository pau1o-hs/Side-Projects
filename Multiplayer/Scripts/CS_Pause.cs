using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class CS_Pause : MonoBehaviour {

    public static bool isOn = false;
    private NetworkManager networkManager;
    public Slider sensitivity, volume;

    private void Start()
    {
        networkManager = NetworkManager.singleton;
        sensitivity.value = PlayerPrefs.GetFloat("Mouse Sensitivity");
        volume.value = PlayerPrefs.GetFloat("Volume");
    }

    private void Update()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
    }

    public void SetSensitivity (float sensetivityValue)
    {
        PlayerPrefs.SetFloat("Mouse Sensitivity", sensetivityValue);
        sensitivity.value = sensetivityValue;

        PlayerPrefs.Save();
    }

    public void SetVolume(float volumeValue)
    {
        PlayerPrefs.SetFloat("Volume", volumeValue);
        volume.value = volumeValue;

        PlayerPrefs.Save();
    }

    public void LeaveRoom ()
    {
        MatchInfo matchInfo = networkManager.matchInfo;
        networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
        networkManager.StopHost();
    }
}
