using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_KillfeedItem : MonoBehaviour {

    [SerializeField] Text text;

    public void Setup (string player, Color playerColor, string source, Color sourceColor)
    {
        text.text = "<b><color=#" + ColorUtility.ToHtmlStringRGBA(sourceColor).ToLower() + ">" + source + "</color></b> <i>killed</i> <b><color=#" + ColorUtility.ToHtmlStringRGBA(playerColor).ToLower() + ">" + player + "</color></b>";
    }
}
