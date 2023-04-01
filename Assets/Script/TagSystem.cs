using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;

public class TagSystem : MonoBehaviour
{
    Rigidbody2D rb;
    public PlayerData pd;
    public GameObject smoke;
    public float exlposionPower = 10f;
    public float explostionTorque = 10f;
    public GameObject lastSmoke;    

    // Start is called before the first frame update
    void Start()
    {
        pd = GetComponent<PlayerData>();
        rb = GetComponent<Rigidbody2D>();
    }
    void OnCollisionEnter2D(Collision2D other) {
        if(!GetComponent<OxygenBoost>().isMine)
            return;

        if(other.transform.tag == "Player")
        {
            if(pd.canChangeTagger == true && pd.isTagger == true){
                
                spawnExplosion(other.contacts[0].point);
                
                float x = Mathf.Round(transform.position.x * 100) / 100;
                float y = Mathf.Round(transform.position.y * 100) / 100;

                float effect_x = Mathf.Round(other.contacts[0].point.x * 100) / 100;
                float effect_y = Mathf.Round(other.contacts[0].point.y * 100) / 100;

                JSON jsonObject = new JSON();
                jsonObject.Add("type","change_tagger");
                jsonObject.Add("oldTagger",transform.name);
                jsonObject.Add("newTagger",other.transform.name);
                jsonObject.Add("x",x);
                jsonObject.Add("y",y);
                jsonObject.Add("effect_x",effect_x);
                jsonObject.Add("effect_y",effect_y);
                TCPManager.instance.SendMsg(jsonObject.CreateString());

                Debug.Log("New Tagger : " + other.gameObject.name );

                pd.isTagger = false;

                // 로컬에서 상대를 술래로 지정하는 것이 아니라
                // 술래는 서버가 바꿔주고 해당 술래의 로컬에서 술래 변경
                // other.gameObject.GetComponent<PlayerData>().changeToTagger();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "explosion")
        {
            if(lastSmoke == null || lastSmoke != other.gameObject)
                lastSmoke = other.gameObject;
                
            oxygenExplosion(lastSmoke.transform.position);
        }
    }

    public void spawnExplosion(Vector2 pos)
    {
        GameObject smokeEffect = Instantiate(smoke);
        smokeEffect.transform.position = pos;
        Destroy(smokeEffect, 1);
    }

    void oxygenExplosion(Vector3 point){
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        
        Vector3 way = transform.position - point;
        
        rb.AddForce(way.normalized * exlposionPower,ForceMode2D.Impulse);
        rb.AddTorque(explostionTorque);
    }

}
