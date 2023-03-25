using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class massObject : MonoBehaviour
{
    public float power = 10f;
    public Vector3 target;
    public GameObject parent;

    void Start()
    {
        Destroy(this.gameObject,1f);
    }

    void Update()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.right * -power);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.transform.tag == "Player")
        {
            Vector2 way = other.transform.position;
            way = transform.position - parent.transform.position;
            other.transform.GetComponent<Rigidbody2D>().AddForce(way.normalized * 2f);
            other.transform.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-2f,2f) * 10f,ForceMode2D.Impulse);

            Destroy(this.gameObject);
        }
    }
}
