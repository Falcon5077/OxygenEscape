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
    public int count = 1;

    private void Start() {
        // spawnPrefab();
    }

    private void Awake() {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void syncObstacle(int index,float x, float y, float z)
    {
        
    }

    public GameObject getNear(Vector3 pos)
    {
        float near = float.MaxValue;
        int minIndex = 0;
        for(int i = 0; i < objects.Count; i++)
        {
            float d = Vector3.Distance(objects[i].transform.position,pos);
            if(near > d)
            {
                near = d;
                minIndex = i;
            }
        }

        return objects[minIndex];
    }
    public void spawnPrefab()
    {
        int index = Random.Range(0,prefabs.Count);

        GameObject t = Instantiate(prefabs[index],Vector3.zero,Quaternion.identity);
        t.name = "Rock";

        objects.Add(t);
    }

    private void sendValue(GameObject g)
    {
        float x = Mathf.Round(g.transform.position.x * 100) / 100;
        float y = Mathf.Round(g.transform.position.y * 100) / 100;
        float z = Mathf.Round(g.transform.eulerAngles.z * 100) / 100;

        JSON jsonObject = new JSON();
        jsonObject.Add("type","sync_obstacle");
        jsonObject.Add("x",x);
        jsonObject.Add("y",y);
        jsonObject.Add("z",z);
        TCPManager.instance.SendMsg(jsonObject.CreateString());
    }
}
