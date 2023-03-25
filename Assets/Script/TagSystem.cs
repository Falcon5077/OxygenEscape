using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagSystem : MonoBehaviour
{
    Rigidbody2D rb;
    public PlayerData ch;
    public GameObject smoke;
    public float exlposionPower = 10f;
    public float explostionTorque = 10f;
    public GameObject lastSmoke;    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void OnCollisionEnter2D(Collision2D other) {
        if(other.transform.tag == "Player")
        {
            if(ch.canChangeTagger == true && ch.isTagger == true){
                GameObject smokeEffect = Instantiate(smoke);
                smokeEffect.transform.position = other.contacts[0].point;
                Destroy(smokeEffect, 1);

                Debug.Log("New Tagger : " + other.gameObject.name );

                ch.isTagger = false;
                other.gameObject.GetComponent<PlayerData>().changeToTagger();
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

    void oxygenExplosion(Vector3 point){
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        
        Vector3 way = transform.position - point;
        
        rb.AddForce(way.normalized * exlposionPower,ForceMode2D.Impulse);
        rb.AddTorque(explostionTorque);
    }
}
