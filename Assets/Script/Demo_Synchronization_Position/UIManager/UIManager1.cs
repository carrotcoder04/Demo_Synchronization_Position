using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static EventGame;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Unity.VisualScripting;
using System.Collections;
using PimDeWitte.UnityMainThreadDispatcher;

public class UIManager1 : MonoBehaviour
{
    [SerializeField] Button register, login;
    [SerializeField] TMP_InputField username, password;
    [SerializeField] Button joinlobby;
    [SerializeField] GameObject joingamepanel;
    private bool isActive = false;
    void Start()
    {
        EventJoinGame = ShowPanelJoinLobby;
        register.onClick.AddListener(() => Register());
        login.onClick.AddListener(() => Login());
        joinlobby.onClick.AddListener(() => JoinLobby());
    }

    void Register()
    {
        string username = this.username.text;
        string password = this.password.text;
        if(username.Length < 6 || password.Length < 6)
        {
            Debug.Log("Tài khoản và mật khẩu phải có tối thiểu 6 kí tự");
            return;
        }
        UserMessage userMessage = new UserMessage(username, password);
        WsClient.Instance.SendByte((byte)Log.REGISTER,userMessage);
        
    }
    void Login()
    {
        string username = this.username.text;
        string password = this.password.text;
        if (username.Length < 6 || password.Length < 6)
        {
            Debug.Log("Tài khoản và mật khẩu phải có tối thiểu 6 kí tự");
            return;
        }
        UserMessage userMessage = new UserMessage(username, password);
        WsClient.Instance.SendByte((byte)Log.LOGIN, userMessage);
    }
    public void ShowPanelJoinLobby()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            PlayerManager.Instance.mainPlayer = username.text;
            joingamepanel.SetActive(true);
        });
    }
    
    void JoinLobby()
    {
        SceneManager.LoadScene(1);
    }
}
