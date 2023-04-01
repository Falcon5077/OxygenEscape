using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PannelController : MonoBehaviour
{
    public List<Transform> childs = new List<Transform>();
    public GameObject gameSceneUI;
    public static PannelController instance;
    public GameObject matchingPannel;
    private void Awake() {
        if(instance == null)
            instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        var allChilds = new List<GameObject>();

        for(int i = 0; i < transform.childCount; i++)
        {
            var item = transform.GetChild(i);
            if(item.transform.parent == this.transform)
                childs.Add(item.transform);
        }
    }

    public void loadGameScene()
    {
        int count = childs.Count;
        for(int i = 0; i < count; i++)
        {
            if(childs[i].name == "MessageFromServer")
                continue;

            childs[i].gameObject.SetActive(false);

        }
        
        gameSceneUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
