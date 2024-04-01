using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventGame;
public class UIManager3 : Singleton<UIManager3>
{
    public Text roomname;
    [SerializeField] Button play;
    [SerializeField] TextMeshProUGUI currentplayer;
    private void Awake()
    {
        Sceen3LoadComplete?.Invoke();
        CurrentPlayer += (currentPlayer) =>
        {
            string result = "";
            foreach (var kvp in currentPlayer)
            {
                result += ("ID: " + kvp.Key.ToString() + ",Name: " + kvp.Value + "\n");
            }
            currentplayer.text = result;
            Debug.Log(result);
        };
        play.onClick.AddListener(() =>
        {
            WsClient.Instance.SendByte((byte)Room.PLAY);
        });
    }
}
