using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static ConvertDirect;
public class Player : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] Animator animator;
    [SerializeField] Transform playerTF;
    bool IsLocalPlayer;
    public byte id;
    public string playername;
    public const int FPS = 60;
    public const float FRAME = 1.0f / FPS;
    public byte direct;
    public string currentAnim = "Idle";
    public Anim _currentAnim;
    public float speed;
    public Queue<Direct> directQueue = new Queue<Direct>();
    public void OnInit(bool IsLocalPlayer,byte id,string name)
    {
        speed = 4f;
        ChangeAnim(Anim.IDLE);
        this.IsLocalPlayer = IsLocalPlayer;
        this.id = id;
        this.playername = name;
        nameTxt.text = name;
        EventGame.OnGameMessage += OnRecieve;
        if(IsLocalPlayer)
        {
            nameTxt.color = Color.red;
            StartCoroutine(LocalPlayerMove());
        }
        else
        {
            nameTxt.color = Color.white;
            StartCoroutine(GlobalPlayerMove());
        }
    }
    IEnumerator LocalPlayerMove()
    {
        while (true)
        {
            Direct Direct = new Direct()
            {
                x = 0,
                y = 0,
                anim = Anim.IDLE
            };
            if (Input.GetKey(KeyCode.A))
            {
                Direct.x = -1;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Direct.x = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                Direct.y = -1;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                Direct.y = 1;
            }
            if (Direct.x != 0 || Direct.y != 0)
            {
                Direct.anim = Anim.RUN;
                direct = DirectToByte(Direct);
                byte[] data = PackTransform(id, direct);
                Send(data);
                Move(Direct);
            }
            else
            {
                if(_currentAnim != Direct.anim)
                {
                    direct = DirectToByte(Direct);
                    byte[] data = PackTransform(id, direct);
                    Send(data);
                    ChangeAnim(Direct.anim);
                }
            }
            yield return new WaitForSeconds(FRAME);
        }
    }
    IEnumerator GlobalPlayerMove()
    {
        while (true)
        {
            float frame = FRAME;
            if (directQueue.Count != 0) frame = FRAME / directQueue.Count;
            while (directQueue.Count > 0)
            {
                var Direct = directQueue.Dequeue();
                Move(Direct);
                yield return new WaitForSeconds(frame);
            }
            yield return new WaitForSeconds(frame);
        }
    }
    void Send(byte[] data)
    {
        WsClient.Instance.SendByte(data);
    }
    void OnRecieve(byte[] data)
    {
        if (data[0] == (byte)Game.PLAYER_POSITION)
        {
            if (data[1] == id)
            {
                var Direct = ByteToDirect(data[2]);
                directQueue.Enqueue(Direct);
            }
        }
    }
    void Move(Direct Direct)
    {
        if(Direct.x == -1)
        {
            playerTF.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if(Direct.x == 1)
        {
            playerTF.rotation = Quaternion.Euler(0, 0, 0);
        }
        transform.position += (new Vector3(Direct.x, Direct.y, 0).normalized * speed * FRAME);
        ChangeAnim(Direct.anim);
    }
    void ChangeAnim(Anim anim)
    {
        _currentAnim = anim;
        string animName = EnumConvertAnim(anim);
        if (currentAnim != animName)
        {
            if (currentAnim != null) animator.ResetTrigger(currentAnim);
            currentAnim = animName;
            animator.SetTrigger(currentAnim);
        }
    }
    string EnumConvertAnim(Anim anim)
    {
        switch (anim)
        {
            case Anim.RUN:
                return "Run";
            case Anim.IDLE:
                return "Idle";
            case Anim.DEATH:
                return "Death";
            case Anim.ATTACK:
                return "Attack";
            default:
                break;
        }
        return string.Empty;
    }
}