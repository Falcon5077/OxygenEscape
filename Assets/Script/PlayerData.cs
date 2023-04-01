using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;
using UnityEngine.SceneManagement;

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
        useOxygen();

        
    }

    void useOxygen()
    {
        if(isTagger && Oxygen > 0){
            Oxygen -= 0.1f * Time.deltaTime;
        }

        if(Oxygen < 0)
        {
            Oxygen = 0;
            Debug.Log("사망");
            
            MessageFromServer.instance.setMessageFromServer(SyncManager.instance.users.Count + "등");
            
            JSON jsonObject = new JSON();
            jsonObject.Add("type","destroy_user");
            TCPManager.instance.SendMsg(jsonObject.CreateString());
            SceneManager.LoadScene("Client");
            UserManager.instance.leaveRoom();
            // PannelController.instance.loadGameScene();
            // PannelController.instance.matchingPannel.SetActive(true);
        }
    }

    void waitDelay()
    {
        isTagger = true;
        canChangeTagger = true;
    }
    public void changeToTagger(){
        canChangeTagger = false;

        speed += speed * 0.2f;
        canChangeTagger = false;
        Invoke("waitDelay",1f);
    }

}
