using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
    public int way = 0;
    float speed = 10;


    // Update is called once per frame
    void Update()
    {
        if(way == 0)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime,Space.World);
            transform.Rotate(Vector3.forward * speed * Time.deltaTime,Space.World);
        }
            
        else
        {
            transform.Translate(Vector3.right * -speed * Time.deltaTime,Space.World);
            transform.Rotate(Vector3.forward * -speed * 10 * Time.deltaTime,Space.World);
        }

        checkPosition();
    }

    void checkPosition()
    {
        if(way == 0)
        {
            if(transform.position.x > 50)
            {
                Destroy(this.gameObject);
                BackGround.instance.spawnPrefab();
            }
        }
        else
        {
            if(transform.position.x < -50)
            {
                Destroy(this.gameObject);
                BackGround.instance.spawnPrefab();
            }
        }
    }
}
