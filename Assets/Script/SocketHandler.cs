using UnityEngine;
using System;
using Leguar.TotalJSON;

public class SocketHandler : MonoBehaviour
{
    public int beforeLength = 0;
    public static SocketHandler instance;
    public GameObject massBall;
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
            else if(type == "sync_obstacle")
            {
                Debug.Log(jsonObject.CreateString());
                string sender = jsonObject.GetString("sender");
                string obj = jsonObject.GetString("index");
                bool value = jsonObject.GetBool("value");
                GameObject Rock = GameObject.Find(obj);

                if(!value)
                {
                    GameObject player = GameObject.Find(sender);

                    if(Rock != null && player != null)
                    {
                        Rock.GetComponent<ObstacleSystem>().canGrab = false;
                        Rock.GetComponent<ObstacleSystem>().target = player;
                    }
                }
                else
                {
                    float x = jsonObject.GetFloat("x");
                    float y = jsonObject.GetFloat("y");
                    float rotateDegree = jsonObject.GetFloat("rotateDegree");

                    Rock.GetComponent<ObstacleSystem>().canGrab = true;    
                    Rock.GetComponent<ObstacleSystem>().target = null;

                    Rock.GetComponent<CapsuleCollider2D>().isTrigger = false;

                    Rock.transform.position = new Vector3(x,y,0);
                    Rock.transform.rotation = Quaternion.Euler(0,0,rotateDegree);
                    Rock.GetComponent<Rigidbody2D>().AddForce(Rock.transform.right * 200,ForceMode2D.Impulse);
                }

                // if(ObstacleSpawner.instance != null)
                //     ObstacleSpawner.instance.syncObstacle(Int32.Parse(sender),x,y,z);
            }
            else if(type == "sync_grab")
            {
                float x = jsonObject.GetFloat("x");
                float y = jsonObject.GetFloat("y");
                float rotateDegree = jsonObject.GetFloat("rotateDegree");
                string sender = jsonObject.GetString("sender");

                GameObject player = GameObject.Find(sender);
        
                GameObject ball = Instantiate(massBall, new Vector3(x,y,0),Quaternion.Euler (0f, 0f, rotateDegree));
                ball.GetComponent<massObject>().parent = player;
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
                    PannelController.instance.NickNamePannel.SetActive(false);
                    
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
                    
                SyncManager.instance.isHost = true;
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

                SyncManager.instance.isHost = false;
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

                if(MessageFromServer.instance != null)
                {
                    string msg = jsonObject.GetString("msg");
                    MessageFromServer.instance.setMessageFromServer(msg);
                }

                // 5초 뒤 태거 갱신
                SyncManager.instance.Invoke("getNewTagger",5f);
            }
            else if(type == "new_tagger")
            {
                if(MessageFromServer.instance != null)
                {
                    string name = jsonObject.GetString("name");
                    MessageFromServer.instance.setMessageFromServer(name + " 술래가 되었습니다.");

                    SyncManager.instance.setNewTagger(name);

                    Debug.Log("+++ 술래 바뀜 : " + name);
                }

            }
            else if(type == "user_list")
            {
                JArray jsonArray = jsonObject.GetJArray("users");
                SyncManager.instance.users.Clear();

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
            else if(type == "set_tagger")
            {
                string target = jsonObject.GetString("newTagger");

                if(SyncManager.instance != null)
                    SyncManager.instance.setTagger(target);
            }
            else if(type == "change_tagger")
            {
                string _old = jsonObject.GetString("oldTagger");
                string _new = jsonObject.GetString("newTagger");

                float _x = jsonObject.GetFloat("x");
                float _y = jsonObject.GetFloat("y");
                float _effect_x = jsonObject.GetFloat("effect_x");
                float _effect_y = jsonObject.GetFloat("effect_y");

                if(SyncManager.instance != null)
                    SyncManager.instance.syncTagger(_old,_new,_x,_y,_effect_x,_effect_y);

                if(MessageFromServer.instance != null)
                    MessageFromServer.instance.setMessageFromServer(_new + " 술래가 되었습니다.");
            }
            else if(type == "close_connection")
            {
                string _name = jsonObject.GetString("name");
                if(SyncManager.instance != null)
                    SyncManager.instance.removeUser(_name);
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
