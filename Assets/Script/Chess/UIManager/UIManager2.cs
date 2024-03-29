using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager2 : MonoBehaviour
{
    [SerializeField] Button createroom, findroom;
    [SerializeField] TMP_InputField idRoom;
    private void Start()
    {
        createroom.onClick.AddListener(() => CreateRoom());
        findroom.onClick.AddListener(() => FindRoom());
    }
    void CreateRoom()
    {
        WsClient.Instance.SendByte((byte)Lobby.CREATE_ROOM);
    }
    void FindRoom()
    {
        string id = idRoom.text;
        LobbyMessage lobbyMessage = new LobbyMessage(id);
        WsClient.Instance.SendByte((byte)Lobby.JOIN_ROOM, lobbyMessage);
    }
}