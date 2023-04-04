using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class radiusEvent : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "wall")
        {
            transform.parent.GetComponent<OxygenSkill>().canShoot = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        
        if(other.tag == "wall")
        {
            transform.parent.GetComponent<OxygenSkill>().canShoot = true;
        }
    }
}
