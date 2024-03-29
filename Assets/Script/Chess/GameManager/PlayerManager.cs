using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class PlayerManager : Singleton<PlayerManager>
{
    Dictionary<byte, string> currentplayer => WsClient.Instance.currentplayer;
    [SerializeField] GameObject playerPrefaps;
    List<Player> players = new List<Player>();
    public string mainPlayer;
    TextMeshProUGUI asyncTF;
    public void CreatePlayer()
    {
        EventGame.PlayerLeaveRoom += PlayerLeaveRoom;
        EventGame.PlayerJoinRoom += PlayerJoinRoom;
        asyncTF = FindFirstObjectByType<TextMeshProUGUI>();
        foreach (var player in currentplayer)
        {
            bool isMe;
            if(mainPlayer == player.Value)
            {
                isMe = true;
            }
            else
            {
                 isMe = false;
            }
            CreatePlayer(isMe,player.Key,player.Value);
            StartCoroutine(AsyncTF());
        }
    }
    IEnumerator AsyncTF()
    {
        while(true)
        {
            string text = "";
            foreach(var player in players)
            {
                text += $"{player.playername} | x: {player.transform.position.x} | y: {player.transform.position.y} | anim: {player.currentAnim}" + "\n";
            }
            asyncTF.text = text;
            yield return null;
        }
    }
    public void CreatePlayer(bool isMe,byte id, string username)
    {
        Player player = Instantiate(playerPrefaps,new Vector3(0,0,0),Quaternion.identity).GetComponent<Player>();
        player.OnInit(isMe,id,username);
        players.Add(player);
    }
    public void RemovePlayer(byte id)
    {
        var player = players.FirstOrDefault(p => p.id.Equals(id));
        Destroy(player.gameObject);
        players.Remove(player);
    }
    void PlayerLeaveRoom(KeyValuePair<byte, string> player)
    {
        RemovePlayer(player.Key);
    }
    void PlayerJoinRoom(KeyValuePair<byte, string> player)
    {
        bool isMe = false;
        if(player.Value == mainPlayer)
        {
            isMe = true;
        }
        CreatePlayer(isMe,player.Key, player.Value);
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
