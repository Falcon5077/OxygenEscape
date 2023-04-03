using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Leguar.TotalJSON;
using UnityEngine.SceneManagement;

public class UserManager : MonoBehaviour
{
    public static UserManager instance;
    public GameObject myObject;
    public GenerateChatMsg generateChatMsg;
    public TextMeshProUGUI userCountText;
    public TextMeshProUGUI roomIdText;
    public InputFieldManager chattingField;
    public GameObject MatchingPannel;
    public GameObject ChattingPannel;
    public string myName;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setMyNameToServer(TMP_InputField nameField)
    {
        myName = nameField.text;

        JSON jsonObject = new JSON();
        jsonObject.Add("type","first_login");
        jsonObject.Add("name",myName);
        TCPManager.instance.SendMsg(jsonObject.CreateString());
    }

    public void playRandomMatching()
    {
        JSON jsonObject = new JSON();
        jsonObject.Add("type","enter_room");
        TCPManager.instance.SendMsg(jsonObject.CreateString());
        Debug.Log(jsonObject.CreateString());
    }

    public void getUserCount()
    {   
        JSON jsonObject = new JSON();
        jsonObject.Add("type","check_user");
        TCPManager.instance.SendMsg(jsonObject.CreateString());
        Debug.Log(jsonObject.CreateString());
    }

    public void getRoomData()
    {
        JSON jsonObject = new JSON();
        jsonObject.Add("type","check_room");
        TCPManager.instance.SendMsg(jsonObject.CreateString());
        Debug.Log(jsonObject.CreateString());
    }
    public void setRoomData(string roomId)
    {
        roomIdText.text = roomId;
        activeRoom(true);
    }
    public void activeRoom(bool active)
    {
        MatchingPannel.SetActive(!active);
        ChattingPannel.SetActive(active);
    }
    public void setUserCount(string count)
    {   
        userCountText.text = count;
    }
    public void sendLeaveRoomToServer()
    {
        if(roomIdText.text == "0")
            return;

        JSON jsonObject = new JSON();
        jsonObject.Add("type","leave_room");
        TCPManager.instance.SendMsg(jsonObject.CreateString());
        Debug.Log(jsonObject.CreateString());
    }
    
    public void leaveRoom()
    {
        activeRoom(false);
        generateChatMsg.clearChat();
        generateChatMsg.setBtnActive(false);
        generateChatMsg.setHostActive(false);

        getUserCount();
    }
    public void sendChattingToRoom()
    {
        string msg = chattingField.getFieldText();

        JSON jsonObject = new JSON();
        jsonObject.Add("type","message_room");
        jsonObject.Add("msg",msg);
        TCPManager.instance.SendMsg(jsonObject.CreateString());
        Debug.Log(jsonObject.CreateString());

        chattingField.setFieldClear();
    }

    public void startGame()
    {
        JSON jsonObject = new JSON();
        jsonObject.Add("type","start_game");
        TCPManager.instance.SendMsg(jsonObject.CreateString());
        Debug.Log(jsonObject.CreateString());
    }

    public void loadGame()
    {
        SceneManager.LoadScene("Game 2");
        
        if(PannelController.instance != null)
            PannelController.instance.loadGameScene();

        getRoomData();
    }

    public void recvChattingFromRoom(string sender, string msg)
    {
        generateChatMsg.spawnChatMsg(sender,msg);
    }
}
