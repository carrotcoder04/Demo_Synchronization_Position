using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WebSocketSharp;
using static EventGame;
using static BinaryConvert;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine.SceneManagement;
using System.Collections.Concurrent;

public class WsClient : Singleton<WsClient>
{
    WebSocket ws = new WebSocket("ws://localhost:8080");
    string idroom;
    public Dictionary<byte,string> currentplayer = new Dictionary<byte, string>();
    private void Awake()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Sceen3LoadComplete = () =>
            {
                Invoke(nameof(Sceen3Loadcomplete), 0.1f);
            };
        });
        DontDestroyOnLoad(gameObject);
        ws.OnMessage += OnLogMessage;
        ws.OnError += OnError;
        ws.OnClose += OnClose;
        ws.Connect();
    }
    void Sceen3Loadcomplete()
    {
        UIManager3.Instance.roomname.text = idroom;
        CurrentPlayer?.Invoke(currentplayer);
    }
    void OnLogMessage(object sender,MessageEventArgs e)
    {
        try
        {
            byte[] data = e.RawData;
            Log info = (Log)data[0];
            if (info == Log.LOGIN_SUCCESS)
            {
                Debug.Log("Login Success");
                SendByte((byte)Log.JOIN_LOBBY);
            }
            else if (info == Log.REGISTER_SUCCESS)
            {
                Debug.Log("Register Success");
                SendByte((byte)Log.JOIN_LOBBY);
            }
            else if (info == Log.USERNAME_OR_PASSWORD_INVALID)
            {
                Debug.Log("Username or password is invalid.");
            }
            else if (info == Log.USERNAME_EXISTED)
            {
                Debug.Log("Username existed");
            }
            else if (info == Log.JOIN_LOBBY_FAIL)
            {
                Debug.Log("Join Lobby Fail");
            }
            else if (info == Log.JOIN_LOBBY_SUCCESS)
            {
                EventJoinGame?.Invoke();
                ws.OnMessage += OnLobbyMessage;
                ws.OnMessage -= OnLogMessage;
            }
            else if(info == Log.YOU_ARE_LOGGED_IN_SOMEWHERE_ELSE)
            {
                Debug.Log("YOU_ARE_LOGGED_IN_SOMEWHERE_ELSE");
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    void OnLobbyMessage(object sender, MessageEventArgs e)
    {
        try
        {
            byte[] data = e.RawData;
            Lobby info = (Lobby)data[0];
            if(info == Lobby.JOIN_ROOM_SUCCESS)
            {
                Debug.Log("JOIN_ROOM_SUCCESS");
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    SceneManager.LoadScene(2);
                });
                ws.OnMessage += OnRoomMessage;
                ws.OnMessage -= OnLobbyMessage;
            }
            else if(info == Lobby.ROOM_NOT_FOUND)
            {
                Debug.Log("ROOM_NOT_FOUND");
            }
            else if(info == Lobby.ROOM_FULL)
            {
                Debug.Log("Room full");
            }
        }
        catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    void OnRoomMessage(object sender, MessageEventArgs e)
    {
        try
        {
            byte[] data = e.RawData;
            Room info = (Room)data[0];
            if (info == Room.ROOM_INFO)
            {
                RoomMessage roomMessage = new RoomMessage(data);
                currentplayer = roomMessage.data;
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    try
                    {
                        CurrentPlayer?.Invoke(currentplayer);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }
                });
            }
            else if (info == Room.ROOM_NAME)
            {
                idroom = ConvertString(data);
            }
            else if (info == Room.PLAY)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    SceneManager.LoadScene(3);
                    //ws.OnMessage -= OnRoomMessage;
                    ws.OnMessage += OnGameMessage;
                });
            }
            else if (info == Room.PLAYER_LEAVE_ROOM)
            {
                RoomUpdate roomUpdate = new RoomUpdate(data);
                currentplayer.Remove(roomUpdate.data.Key);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                 {
                     PlayerLeaveRoom?.Invoke(roomUpdate.data);
                     CurrentPlayer?.Invoke(currentplayer);
                 });
            }
            else if (info == Room.NEW_PLAYER_JOIN_ROOM)
            {
                RoomUpdate roomUpdate = new RoomUpdate(data);
                currentplayer[roomUpdate.data.Key] = roomUpdate.data.Value;
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    PlayerLeaveRoom?.Invoke(roomUpdate.data);
                    CurrentPlayer?.Invoke(currentplayer);
                });
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void OnGameMessage(object sender, MessageEventArgs e)
    {
        try 
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                EventGame.OnGameMessage?.Invoke(e.RawData);
            });
        }
        catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    void OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log(e.Reason);
    }
    void OnError(object sender, ErrorEventArgs e)
    {
        Debug.Log($"Error: {e.Message}");
    }
    public void SendByte(byte tag, byte[] data)
    {
        data[0] = tag;
        ws.Send(data);
    }
    public void SendByte(byte tag, byte isSuccess)
    {
        byte[] data = new byte[2];
        data[0] = tag;
        data[1] = isSuccess;
        ws.Send(data);
    }
    public void SendByte(byte tag, string data)
    {
        byte[] datas = Encoding.UTF8.GetBytes(data);
        datas = AddFirst(tag, datas);
        ws.Send(datas);
    }
    public void SendByte(byte tag)
    {
        byte[] data = new byte[] { tag };
        ws.Send(data);
    }
    public void SendByte(byte[] data)
    {
        ws.Send(data);
    }
    public void SendByte(byte tag, ISerializable data)
    {
        byte[] bytes = data.ConvertByte();
        bytes[0] = tag;
        ws.Send(bytes);
    }
}