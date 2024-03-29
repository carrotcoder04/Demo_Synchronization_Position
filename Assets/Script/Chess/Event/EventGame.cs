using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine.Events;

public static class EventGame
{
    public static UnityAction EventJoinGame;
    public static UnityAction<Dictionary<byte,string>> CurrentPlayer;
    public static UnityAction<KeyValuePair<byte,string>> PlayerLeaveRoom;
    public static UnityAction<KeyValuePair<byte,string>> PlayerJoinRoom;
    public static UnityAction Sceen3LoadComplete;
    public static UnityAction<byte[]> OnGameMessage;
}