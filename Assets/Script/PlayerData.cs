using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public bool isTagger = false;
    public bool canChangeTagger = true;
    public float Oxygen = 1000f;
    public float Stamina = 100f;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        if(isTagger){
            Oxygen -= 0.1f * Time.deltaTime;
        }
        
    }

    void waitDelay()
    {
        canChangeTagger = true;
    }

    public void changeToTagger(){
        isTagger = true;
        speed += speed * 0.2f;
        canChangeTagger = false;
        Invoke("waitDelay",1f);
    }

}
