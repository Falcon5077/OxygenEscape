using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    public bool isTagger = false;
    public bool canChangeTagger = true;
    public float Oxygen = 100f;
    public float Stamina = 100f;
    public float speed;
    public ClientObject co;

    // Update is called once per frame
    void Update()
    {
        useOxygen();
    }

    private void Awake() {
        co = GetComponent<ClientObject>();
    }

    void useOxygen()
    {
        if(isTagger && Oxygen > 0){
            Oxygen -= 0.1f * Time.deltaTime;
        }

        if(!co.isMine)
            return;

        if(isTagger)
        {
            // 술래인 시간 더하기
            ResultSystem.instance.changeTaggerTimer += Time.deltaTime;
        }
        else if (!isTagger)
        {
            // 술래가 아닌 시간 더하기
            ResultSystem.instance.noneTaggerTimer += Time.deltaTime;
        }

        if(Oxygen < 0)
        {
            Oxygen = 0;
            Debug.Log("사망");

            GameOver();
            
        }
    }

    public void GameOver()
    {
        ResultSystem.instance.setResultText(SyncManager.instance.users.Count.ToString());
        
        JSON jsonObject = new JSON();
        jsonObject.Add("type","destroy_user");
        TCPManager.instance.SendMsg(jsonObject.CreateString());
        SceneManager.LoadScene("Client");
        UserManager.instance.leaveRoom();
    }

    void waitDelay()
    {
        ResultSystem.instance.getNoneBestTime();

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
