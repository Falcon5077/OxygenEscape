using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        TCPManager.instance.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            Vector3 pos = target.transform.position;
            pos.z = -10f;
            transform.position = pos;
        }
        
    }
}
