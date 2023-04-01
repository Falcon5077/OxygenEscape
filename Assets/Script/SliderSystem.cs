using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSystem : MonoBehaviour
{
    public static SliderSystem instance;
    public Slider staminaSlider;
    public Slider oxygenSlider;
    public Image oxygenFill;
    public PlayerData pd;
    float fSliderBarTime;
    void Awake()
    {
        if(instance == null)
            instance = this;
    }
 
    void Update()
    {
        if(pd == null)
            return;

        staminaSlider.value = pd.Stamina;
        oxygenSlider.value = pd.Oxygen;

        checkValue(staminaSlider);
        checkValue(oxygenSlider);
        
        if(pd.isTagger == true)
        {
            oxygenFill.color = Color.red;
        }
        else
        {
            oxygenFill.color = Color.green;
        }
    }

    public void checkValue(Slider s)
    {
        if (s.value <= 0)
            s.transform.Find("Fill Area").gameObject.SetActive(false);
        else
            s.transform.Find("Fill Area").gameObject.SetActive(true);
    }
}
