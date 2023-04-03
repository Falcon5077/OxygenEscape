using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ResultSystem : MonoBehaviour
{
    public static ResultSystem instance;
    public float oxygenTimer = 0f;
    public float noneTaggerTimer = 0f;
    public float noneBest = 0f;
    public float changeTaggerTimer = 0f;
    public float taggerBest = 0f;

    public GameObject resultPannel;

    public TextMeshProUGUI oxygenText;
    public TextMeshProUGUI noneTaggerText;
    public TextMeshProUGUI changeTaggerText;
    public TextMeshProUGUI rankText;

    private void Awake() {
        if(instance == null)
            instance = this;
    }

    public void getNoneBestTime()
    {
        if(noneTaggerTimer > noneBest)
        {
            noneBest = noneTaggerTimer;
            noneTaggerTimer = 0f;
        }
    }
    
    public void getTaggerBestTime()
    {
        if(changeTaggerTimer > taggerBest)
        {
            taggerBest = changeTaggerTimer;
            changeTaggerTimer = 0f;
        }
    }
    
    public void setResultText(string num)
    {
        resultPannel.SetActive(true);
        getNoneBestTime();

        oxygenText.text = "산소 분출한 모든 시간은 " + Mathf.Round(oxygenTimer).ToString() + "초!";
        noneTaggerText.text = "최고 " + Mathf.Round(noneBest).ToString() + "초 동안 산소통을 보유했어요!";
        changeTaggerText.text = "최고 " + Mathf.Round(taggerBest).ToString() + "초 만에 산소통을 뺏었어요!";
        rankText.text = num + "등";

        clearTimer();
    }
    

    public void clearTimer()
    {
        oxygenTimer = 0f;
        noneTaggerTimer = 0f;
        changeTaggerTimer = 0f;
        noneBest = 0f;
        taggerBest = 0f;
    }
}
