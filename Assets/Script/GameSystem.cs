using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    private PlayerInfo[] cList;
    public PlayerInfo ch;
    private int count;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void breakdown(){
        // int who = Random.Range(0, count - 1);
        // cList[who].isTrue = false;
        ch.isTagger = true;
    }

    void gameOver(){
        // 서버로 죽었다는 메시지 날려줘야함
    }
}
