using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : Singleton<GameManager>
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);   
    }
    public void StartRoom()
    {
        PlayerManager.Instance.CreatePlayer();
    }
}
