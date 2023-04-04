using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;

public class SyncManager : MonoBehaviour
{
    public static SyncManager instance;
    public GameObject playerPrefab;
    public List<GameObject> users = new List<GameObject>();
    public List<Vector3> spawnList= new List<Vector3>();

    public Vector3 spawnPos;
    public bool isHost = false;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void getNewTagger()
    {
        JSON jsonObject = new JSON();
        jsonObject.Add("type","get_tagger");
        TCPManager.instance.SendMsg(jsonObject.CreateString());
    }

    public void setNewTagger(string name)
    {
        Debug.Log(users.Count);
        for(int i = 0; i < users.Count; i++)
        {
            Debug.Log(users[i].name + ", " + name);
            if(users[i].name == name)
            {
                // MessageFromServer.instance.setMessageFromServer("New Tagger is " + users[i].name);
                users[i].GetComponent<PlayerData>().isTagger = true;
                break;
            }
        }
    }
    public void syncPosition(string target, bool isInject, float x, float y, float z,int injectWay,float injectionAmount,float injectPointY,float rotateDegree)
    {
        GameObject targetObject = null;
        for(int i = 0; i < users.Count; i++)
        {
            if(users[i].name == target){
                targetObject = users[i];
                break;
            }
        }

        if(targetObject != null)
            targetObject.GetComponent<OxygenBoost>().recvValue(isInject,x,y,z,injectWay, injectionAmount, injectPointY, rotateDegree);
    }

    public GameObject spawnUser(string name,int index)
    {
        spawnPos = spawnList[index];
        GameObject u = Instantiate(playerPrefab,spawnPos,Quaternion.identity);
        u.transform.name = name;
        u.GetComponent<ClientObject>().setUserName(name);
        users.Add(u);

        u.SetActive(true);

        return u;
    }

    public void removeUser(string target)
    {
        for(int i = 0; i < users.Count; i++)
        {
            if(users[i].name == target){
                Destroy(users[i].gameObject);
                users.RemoveAt(i);
                users.TrimExcess();
            }
        }

        if(users.Count == 1)
        {
            users[0].GetComponent<PlayerData>().GameOver();
        }
    }
    public void setTagger(string target)
    {
        for(int i = 0; i < users.Count; i++)
        {
            if(users[i].name == target)
            {
                if(users[i].GetComponent<ClientObject>().isMine)
                {
                    users[i].GetComponent<PlayerData>().changeToTagger();
                    break;
                }
            }
        }
    }
    public void syncTagger(string _old,string _new,float _x,float _y,float _effect_x,float _effect_y)
    {
        for(int i = 0; i < users.Count; i++)
        {
            if(users[i].name == _old)
            {
                Vector3 pos = new Vector3(_x,_y,0);
                users[i].GetComponent<OxygenBoost>().pos = pos;
                users[i].transform.position = pos;
            }

            if(users[i].name == _new)
            {
                Vector2 point = new Vector2(_effect_x,_effect_y);
                users[i].GetComponent<TagSystem>().spawnExplosion(point);

                if(users[i].GetComponent<ClientObject>().isMine)
                {
                    users[i].GetComponent<PlayerData>().changeToTagger();
                }
            }
        }

    }
    
}
