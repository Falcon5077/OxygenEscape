using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObstacleSystem : MonoBehaviour
{
    Rigidbody2D rb;
    
    public Vector3 pos = Vector3.zero;
    public Vector3 rota = Vector3.zero;
    public float lerpSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        MovingNonGravity();
    }

    void MovingNonGravity(){
        rb.AddForce(new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), 0) * 0.5f);
        rb.AddTorque(1, ForceMode2D.Force);
    }

    void Update()
    {
        if(!SyncManager.instance.isHost)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            
            transform.position = Vector3.Lerp(transform.position,pos,Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(rota),Time.deltaTime * lerpSpeed);
            return;
        }

        if(rb.velocity.magnitude < 3){
            MovingNonGravity();
        }
    }

}
