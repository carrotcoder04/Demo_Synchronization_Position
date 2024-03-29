using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static BinaryConvert;
//Enum
public enum Log : byte
{
    USERNAME_OR_PASSWORD_INVALID = 0,
    LOGIN = 1,
    REGISTER = 2,
    LOGOUT = 3,
    JOIN_LOBBY = 4,
    LOGIN_SUCCESS = 5,
    REGISTER_SUCCESS = 6,
    USERNAME_EXISTED = 7,
    JOIN_LOBBY_FAIL = 8,
    JOIN_LOBBY_SUCCESS = 9,
    YOU_ARE_LOGGED_IN_SOMEWHERE_ELSE = 10
}
public enum Lobby : byte
{
    ROOM_NOT_FOUND = 0,
    JOIN_ROOM = 1,
    CREATE_ROOM = 2,
    JOIN_ROOM_SUCCESS = 3,
    ROOM_FULL = 4
}
public enum Room : byte
{
    ERROR = 0,
    ROOM_INFO = 1,
    ROOM_CHAT = 2,
    ROOM_NAME = 3,
    PLAY = 4,
    NEW_PLAYER_JOIN_ROOM = 5,
    PLAYER_LEAVE_ROOM = 6
}
public enum Game : byte
{
    GAME_INFO = 7,
    PLAYER_POSITION = 8
}

public enum Anim : byte
{
    IDLE = 0,
    RUN = 1,
    ATTACK = 2,
    DEATH = 3
}
public struct Direct
{
    public sbyte x;
    public sbyte y;
    public Anim anim;
}
//Interface
public interface ISerializable
{
    byte[] ConvertByte();
    void ConvertClass(byte[] data);
}
//Class Message
public class UserMessage : ISerializable
{
    public string username { get; set; }
    public string password { get; set; }
    public UserMessage(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
    public UserMessage(byte[] bytes)
    {
        ConvertClass(bytes);
    }

    public byte[] ConvertByte()
    {
        byte[] username = Encoding.UTF8.GetBytes(this.username);
        byte[] password = Encoding.UTF8.GetBytes(this.password);
        byte userlength = (byte)username.Length;
        byte passlength = (byte)password.Length;
        username = AddFirst(255, username);
        username = Add(username, password);
        username = Append(username, userlength);
        username = Append(username, passlength);
        return username;
    }
    public void ConvertClass(byte[] data)
    {
        byte passlength = data[data.Length - 1];
        byte userlength = data[data.Length - 2];
        this.username = Encoding.UTF8.GetString(data, 1, userlength + 1);
        this.password = Encoding.UTF8.GetString(data, userlength + 1, passlength);
    }
}
public class LobbyMessage : ISerializable
{
    public string idroom { get; set; }
    public LobbyMessage(string idroom)
    {
        this.idroom = idroom;
    }
    public LobbyMessage(byte[] bytes)
    {
        ConvertClass(bytes);
    }
    public byte[] ConvertByte()
    {
        byte[] idroom = Encoding.UTF8.GetBytes(this.idroom);
        idroom = AddFirst(255, idroom);
        return idroom;
    }

    public void ConvertClass(byte[] data)
    {
        this.idroom = Encoding.UTF8.GetString(data, 1, data.Length - 1);
    }
}
public class RoomMessage : ISerializable
{
    public Dictionary<byte, string> data;
    public RoomMessage(Dictionary<byte, string> data)
    {
        this.data = data;
    }
    public RoomMessage(byte[] bytes)
    {
        data = new Dictionary<byte, string>();
        ConvertClass(bytes);
    }
    public byte[] ConvertByte()
    {
        byte[] result = new byte[] { 255 };
        byte length = (byte)data.Count;
        byte[] namelength = new byte[length];
        byte index = 0;
        foreach (var i in data.Values)
        {
            byte[] name = Encoding.UTF8.GetBytes(i);
            namelength[index] = (byte)name.Length;
            result = Add(result, name);
            index++;
        }
        foreach (var i in data.Keys)
        {
            result = Append(result, i);
        }
        for (byte i = 0; i < length; i++)
        {
            result = Append(result, namelength[i]);
        }
        result = Append(result, length);
        return result;
    }
    public void ConvertClass(byte[] data)
    {
        byte length = data[data.Length - 1];
        byte indexStart = (byte)(data.Length - length - 1);
        byte[] len = new byte[length];
        byte[] id = new byte[length];
        List<string> name = new List<string>();
        for (byte i = indexStart; i < data.Length - 1; i++)
        {
            len[i - indexStart] = data[i];
            id[i - indexStart] = data[i - length];
        }
        byte index = 1;
        for (byte i = 0; i < length; i++)
        {
            this.data.Add(id[i], Encoding.UTF8.GetString(data, index, len[i]));
            index += len[i];
        }
    }
}
public class RoomUpdate : ISerializable
{
    public KeyValuePair<byte, string> data;
    public RoomUpdate(KeyValuePair<byte, string> data)
    {
        this.data = data;
    }
    public RoomUpdate(byte[] data)
    {
        ConvertClass(data);
    }
    public byte[] ConvertByte()
    {
        byte[] result = new byte[] { 255 };
        byte[] name = Encoding.UTF8.GetBytes(data.Value);
        byte namelenth = (byte)name.Length;
        result = Add(result, name);
        result = Append(result, namelenth);
        result = Append(result, data.Key);
        return result;
    }
    public void ConvertClass(byte[] data)
    {
        byte key = data[data.Length - 1];
        byte namelength = data[data.Length - 2];
        string value = Encoding.UTF8.GetString(data, 1, namelength);
        this.data = new KeyValuePair<byte, string>(key, value);
    }
}

