using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldManager : MonoBehaviour
{
    public TMP_InputField field;
    // Start is called before the first frame update
    void Awake()
    {
        if(field == null)
            field = GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string getFieldText()
    {
        return field.text;
    }
    public void setFieldClear()
    {
        field.text = "";
    }
    public void textChange(Button btn)
    {
        if(field.text == "")
        {
            btn.interactable = false;
        }
        else
        {
            btn.interactable = true;
        }
    }
}
