using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageFromServer : MonoBehaviour
{
    public static MessageFromServer instance;
    public TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void setMessageFromServer(string msg)
    {
        StopCoroutine("fadeOut");
        tmp.color = new Color(1,1,1,1);
        tmp.text = msg;
        StartCoroutine("fadeOut");
    }

    public IEnumerator fadeOut()
    {
        yield return new WaitForSeconds(2f);
        float t = 1.0f;
        while(t > 0)
        {
            t = Time.deltaTime;
            tmp.color = new Color(1,1,1,t);
            yield return new WaitForSeconds(0.01f);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
