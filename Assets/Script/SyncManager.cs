using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncManager : MonoBehaviour
{
    public static SyncManager instance;
    public GameObject playerPrefab;
    public List<GameObject> users = new List<GameObject>();
    public Vector3 spawnPos;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
            instance = this;
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
        spawnPos = new Vector3(10f * index ,5f,0);
        GameObject u = Instantiate(playerPrefab,spawnPos,Quaternion.identity);
        u.transform.name = name;
        u.GetComponent<ClientObject>().setUserName(name);
        users.Add(u);

        u.SetActive(true);

        return u;
    }
    
}
