using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGame : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.StartRoom();
    }
}
