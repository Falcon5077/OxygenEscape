using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSystem : MonoBehaviour
{
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        MovingNonGravity();
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.velocity.magnitude < 3){
            MovingNonGravity();
        }
        
    }

    void MovingNonGravity(){
        Debug.Log("뎀프시롤!!!!!!!!!!!!!!!");
        rb.AddForce(new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), 0) * 0.5f);
        rb.AddTorque(10, ForceMode2D.Force);
    }
}
