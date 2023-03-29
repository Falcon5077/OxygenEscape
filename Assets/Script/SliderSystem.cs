using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSystem : MonoBehaviour
{
    Slider slHP;
    public PlayerData pd;
    float fSliderBarTime;
    void Start()
    {
       slHP = GetComponent<Slider>();
    }
 
    void Update()
    {
        if(pd == null)
            return;

        if(gameObject.tag == "stamina"){
            slHP.value = pd.Stamina * 0.01f;
        } else if(gameObject.tag == "oxygen"){
            slHP.value = pd.Oxygen * 0.01f;
        }
        
        if (slHP.value <= 0)
            transform.Find("Fill Area").gameObject.SetActive(false);
        else
            transform.Find("Fill Area").gameObject.SetActive(true);
    }
}
