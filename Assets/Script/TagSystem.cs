using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagSystem : MonoBehaviour
{
    Rigidbody2D rb;
    public Charactor ch;
    public GameObject smoke;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.GetComponent<Charactor>().isTagger == true){
            ch.isTagger = true;
            other.gameObject.GetComponent<Charactor>().isTagger = false;
            oxygenExplosion();
        }
    }

    void oxygenExplosion(){
        GameObject smokeEffect = Instantiate(smoke);
        smokeEffect.transform.position = ch.transform.position;
        Destroy(smokeEffect, 1);
        Debug.Log("TAG!!!!!");
        // rb.AddTorque(50, ForceMode2D.Force);
        // Vector3 vector = Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.forward) * Vector3.right;
        rb.AddForce(new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f)) * 1, ForceMode2D.Impulse);
        // rb.velocity = Vector2.up * 10;
    }
}
