using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Killfeed : MonoBehaviour {

    [SerializeField] GameObject killfeedItemPrefab;

	// Use this for initialization
	void Start () {

        CS_GameManager.instance.onPlayerKilledCallback += OnKill;
	}

    public void OnKill (string player, Color playerColor, string source, Color sourceColor)
    {
        GameObject go = Instantiate(killfeedItemPrefab, this.transform);
        go.GetComponent<CS_KillfeedItem>().Setup(player, playerColor, source, sourceColor);
        Destroy(go, 5);
    }
}
