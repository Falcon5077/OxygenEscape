using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class ClientObject : MonoBehaviour
{
    public TextMeshPro userName;
    public bool isMine = false;

    public void setUserName(string name)
    {
        userName.text = name;
    }

    public void setMine()
    {
        Invoke("delaySetMine",1f);
    }

    public void delaySetMine()
    {
        GetComponent<OxygenBoost>().isMine = true;
        GetComponent<OxygenBoost>().StartCoroutine("sendPacket");
        
        isMine = true;
        Camera.main.GetComponent<CameraFollow>().target = this.gameObject;

        GameSystem.instance.initSpaceStation(7);   
        UserManager.instance.myObject = this.gameObject;
    }
}
