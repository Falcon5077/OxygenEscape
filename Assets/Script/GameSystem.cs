using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static GameSystem instance;
    private PlayerData[] cList;
    public PlayerData pd;
    public GameObject spaceStation;
    public List<GameObject> spaceStations = new List<GameObject>();
    public int[] wasdX = {80, -80, 0, 0};
    public int[] wasdY = {0, 0, 80, -80};
    private int count;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake() {
        if(instance == null)
            instance = this;
    }

    void breakdown(){
        if(pd != null)
            pd.isTagger = true;
    }

    public void initSpaceStation(int count){
        for(int i = 0; i < count; i++){
            GameObject newStation = Instantiate(spaceStation);
            spaceStations.Add(newStation);
            int j = i % 4;
            int k = (i / 4) + 1;
            // newStation.transform.position = new Vector3(0 + wasdX[j], 0 + wasdY[j], 0);
            newStation.transform.position = new Vector3(0 + wasdX[j] * k, 0 + wasdY[j] * k, 0);
            Debug.Log("i:" + i + "j:" + j);
        }
    }

    void destroyStation(int index){
        Destroy(spaceStations[index]);
    }

    void gameOver(){
        // 서버로 죽었다는 메시지 날려줘야함
    }
}
