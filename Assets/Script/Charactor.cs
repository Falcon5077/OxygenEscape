using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charactor : MonoBehaviour
{
    public bool isTagger = false;
    public float Oxygen = 1000f;
    public float Stamina = 100f;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isTagger){
            Oxygen -= 0.1f * Time.deltaTime;
        }
        
    }

    void changeToTagger(){
        isTagger = true;
        speed += speed * 0.2f;
    }

}
