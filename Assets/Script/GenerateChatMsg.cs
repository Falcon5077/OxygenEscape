using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GenerateChatMsg : MonoBehaviour
{
    public GameObject startBtn;
    public Transform chatScroll;
    public List<GameObject> mTexts;
    public GameObject textPrefab;

    public void setBtnActive(bool value)
    {
        startBtn.SetActive(value);
    }
    public void setHostActive(bool value)
    {
        startBtn.GetComponent<Button>().interactable = value;
    }
    public void spawnChatMsg(string sender, string msg)
    {
        GameObject textObj = Instantiate(textPrefab,Vector3.zero,Quaternion.identity,chatScroll);
        string text = "[" + sender + "] : " + msg;
        textObj.GetComponent<TextMeshProUGUI>().text = text;
        mTexts.Add(textObj);

        Debug.Log(text);
    }

    public void clearChat()
    {
        for(int i = 0; i < mTexts.Count; i++)
        {
            Destroy(mTexts[i]);
        }
        mTexts.Clear();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
