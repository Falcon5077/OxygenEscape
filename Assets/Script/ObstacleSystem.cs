using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObstacleSystem : MonoBehaviour
{
    public bool canGrab = true;
    public GameObject target;
        Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update() {
        if(target != null)
            transform.position = target.transform.position;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        MovingNonGravity();
    }
    void MovingNonGravity(){
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        rb.AddForce(new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), 0) * 0.5f);
        rb.AddTorque(1, ForceMode2D.Force);
    }
}
