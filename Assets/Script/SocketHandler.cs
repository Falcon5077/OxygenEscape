using UnityEngine;
using System;
using Leguar.TotalJSON;

public class SocketHandler : MonoBehaviour
{
    public int beforeLength = 0;
    public static SocketHandler instance;

    void Awake()
    {
        if(instance == null)
            instance = this;
    }
    public void Handle_Json_Event(string jsonAsString)
    {
        
        if(jsonAsString == "" || jsonAsString == null || string.IsNullOrWhiteSpace(jsonAsString))
            return;

        try{
            JSON jsonObject = JSON.ParseString(jsonAsString);

            

            string type = jsonObject.GetString("type");

            if(type == "true")
            {
                return;
            }
            else if(type == "sync_position")
            {
                bool isInject = jsonObject.GetBool("isInject");
                float x = jsonObject.GetFloat("x");
                float y = jsonObject.GetFloat("y");
                float z = jsonObject.GetFloat("z");
                int injectWay = jsonObject.GetInt("injectWay");
                float injectionAmount = jsonObject.GetFloat("injectionAmount");
                float injectPointY = jsonObject.GetFloat("injectPointY");
                float rotateDegree = jsonObject.GetFloat("rotateDegree");

                string target = jsonObject.GetString("sender");
                
                SyncManager.instance.syncPosition(target,isInject,x,y,z,injectWay,injectionAmount, injectPointY,rotateDegree);
            }
            else if(type == "false")
            {
                if(LoadingAnim.instance != null)
                    LoadingAnim.instance.stopLoading();
                return;
            }
            else if(type == "setting_name")
            {
                if(LoadingAnim.instance != null)
                    LoadingAnim.instance.stopLoading();

                if(PannelController.instance != null)
                {
                    PannelController.instance.matchingPannel.SetActive(true);
                    
                    if(UserManager.instance != null)
                        UserManager.instance.getUserCount();
                }
            }
            else if(type == "check_user")
            {
                string userCount = jsonObject.GetString("user_count");
                if(UserManager.instance != null)
                    UserManager.instance.setUserCount(userCount);
            }
            else if(type == "enter_success")
            {
                string roomId = jsonObject.GetString("room_id");
                if(UserManager.instance != null)
                    UserManager.instance.setRoomData(roomId);
                if(LoadingAnim.instance != null)
                    LoadingAnim.instance.stopLoading();
            }
            else if(type == "room_host")
            {
                if(UserManager.instance != null)
                    UserManager.instance.generateChatMsg.setBtnActive(true);
            }
            else if(type == "leave_success")
            {
                string value = jsonObject.GetString("value");
                if(value == "false")
                    return;
                    
                if(UserManager.instance != null)
                    UserManager.instance.leaveRoom();
                if(LoadingAnim.instance != null)
                    LoadingAnim.instance.stopLoading();
            }
            else if(type == "guest_enter")
            {
                string sender = "System";
                string name = jsonObject.GetString("name");
                string guestCount = jsonObject.GetString("guest_count");
                string msg = name + "님이 입장하셨습니다. [" + guestCount + "]";
                
                string value = jsonObject.GetString("value");

                if(UserManager.instance != null){
                    UserManager.instance.recvChattingFromRoom(sender,msg);
                    if(value == "true")
                    {
                        msg = "방장은 게임을 시작해주세요.";
                        UserManager.instance.recvChattingFromRoom(sender,msg);
                        UserManager.instance.generateChatMsg.setHostActive(true);
                    }
                }
            }
            else if(type == "guest_exit")
            {
                string sender = "System";
                string name = jsonObject.GetString("name");
                string guestCount = jsonObject.GetString("guest_count");
                string msg = name + "님이 퇴장하셨습니다. [" + guestCount + "]";
                
                if(UserManager.instance != null)
                {
                    UserManager.instance.recvChattingFromRoom(sender,msg);
                    UserManager.instance.generateChatMsg.setHostActive(false);
                }
            }
            else if(type == "load_game")
            {
                if(UserManager.instance != null)
                    UserManager.instance.loadGame();
            }
            else if(type == "user_list")
            {
                JArray jsonArray = jsonObject.GetJArray("users");

                for(int i = 0; i < jsonArray.Length; i++)
                {
                    if(SyncManager.instance != null)
                    {
                        var element = jsonArray.GetString(i);
                        GameObject g = SyncManager.instance.spawnUser(element,i);

                        if(UserManager.instance.myName == element)
                            g.GetComponent<ClientObject>().setMine();

                        Debug.Log(i+"번 : " + element);
                    }
                }
            }
            else if(type == "client_msg")
            {
                string sender = jsonObject.GetString("sender");
                string msg = jsonObject.GetString("msg");

                if(UserManager.instance != null)
                    UserManager.instance.recvChattingFromRoom(sender,msg);
            }

            beforeLength = jsonAsString.Length;
        }
        catch (Exception e){
            Debug.Log(jsonAsString.Length);
            Debug.Log("Error : " + e);
            // TCPManager.instance.StartCoroutine("clearPacket");
            
            return;
        }
    }

}
