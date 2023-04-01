using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public static BackGround instance;
    public List<Sprite> prefabs = new List<Sprite>();
    public GameObject prefab;

    public float max_x;
    public float max_y;
    public int way = 0; 
    // Start is called before the first frame update
    void Awake()
    {

        for(int i = 0; i < 15; i++)
            Invoke("spawnPrefab",i);

        for(int i = 0; i < 5; i++)
            spawnPrefab();

        if(instance == null)
            instance = this;
    }

    public void spawnPrefab()
    {
        way = Random.Range(0,2);
        GameObject temp = Instantiate(prefab,Vector3.zero,Quaternion.identity);

        int index = Random.Range(0,8);
        temp.GetComponent<SpriteRenderer>().sprite = prefabs[index];
        temp.GetComponent<SpinObject>().way = way;

        if(way == 0)
        {
            float x = Random.Range(0,max_x);
            float y = Random.Range(-max_y,max_y);
            temp.transform.position = new Vector3(-50 + Random.Range(-5,5),y,0);
        }
        else
        {
            float x = Random.Range(0,-max_x);
            float y = Random.Range(-max_y,max_y);
            temp.transform.position = new Vector3(50 + Random.Range(-5,5),y,0);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
