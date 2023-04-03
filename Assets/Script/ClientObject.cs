using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class ClientObject : MonoBehaviour
{
    public TextMeshPro userName;
    public bool isMine = false;
    public GameObject ball;

    public void setUserName(string name)
    {
        userName.text = name;
    }

    public void setMine()
    {
        isMine = true;
        Camera.main.GetComponent<CameraFollow>().target = this.gameObject;
        SliderSystem.instance.pd = GetComponent<PlayerData>();
        Invoke("delaySetMine",1f);
    }

    public void delaySetMine()
    {
        GetComponent<OxygenBoost>().isMine = true;
        GetComponent<OxygenBoost>().StartCoroutine("sendPacket");

        // GameSystem.instance.initSpaceStation(7);   
        UserManager.instance.myObject = this.gameObject;
    }
}
