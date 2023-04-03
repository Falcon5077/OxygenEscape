using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;

public class ObstacleSpawner : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>();
    public List<GameObject> prefabs = new List<GameObject>();

    public bool isSend = false;
    public static ObstacleSpawner instance;

    private void Start() {
        for(int i = 0; i < 10; i++)
        {
            spawnPrefab(i);
        }
    }

    private void Awake() {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Update() {
        if(SyncManager.instance.isHost)
        {
            // Send
            if(isSend == false)
            {
                isSend = true;
                StartCoroutine("sendPacket");
            }
        }
    }

    public void syncObstacle(int index,float x, float y, float z)
    {
        objects[index].GetComponent<ObstacleSystem>().pos = new Vector3(x,y,0);
        objects[index].GetComponent<ObstacleSystem>().rota = new Vector3(0,0,z);
    }

    public void spawnPrefab(int i)
    {
        int index = Random.Range(0,prefabs.Count);

        GameObject t = Instantiate(prefabs[index],Vector3.zero,Quaternion.identity);
        t.name = i.ToString();

        objects.Add(t);
    }

    IEnumerator sendPacket()
    {
        while(SyncManager.instance.isHost)
        {
            for(int i = 0; i < 10; i++)
                sendValue(objects[i]);
                
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void sendValue(GameObject g)
    {
        float x = Mathf.Round(g.transform.position.x * 100) / 100;
        float y = Mathf.Round(g.transform.position.y * 100) / 100;
        float z = Mathf.Round(g.transform.eulerAngles.z * 100) / 100;

        JSON jsonObject = new JSON();
        jsonObject.Add("type","sync_obstacle");
        jsonObject.Add("sender",g.name);
        jsonObject.Add("x",x);
        jsonObject.Add("y",y);
        jsonObject.Add("z",z);
        TCPManager.instance.SendMsg(jsonObject.CreateString());
    }
}
