using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class LoadingAnim : MonoBehaviour
{
    public static LoadingAnim instance;
    public int numObjects = 10;
    public GameObject prefab;
    public List<Image> mLists = new List<Image>();
    public Vector3 center;
    public Transform parent;
    public Vector2 bigPos = new Vector2(30,30);
    public Vector2 endPos = new Vector2(20,20);
    public Vector2 smallPos = new Vector2(10,10);
    public float radius;
    public float speed = 0.1f;
    public int count = 0;

    private void Awake() {
        if(instance == null)
            instance = this;
    }
    private void OnEnable() {

        for(int i = 0; i < mLists.Count; i++)
        {
            mLists[i].GetComponent<RectTransform>().sizeDelta = endPos;
        }

        count = 0;
        loadingStart();
    }

    public void loadingStart()
    {
        Debug.Log("Start coroutine");
        StartCoroutine("loading");
    }

    private void OnDisable() {
        Debug.Log("Stop coroutine");
        StopCoroutine("loading");
        StopAllCoroutines();
    }
    
    IEnumerator loading()
    {
        if(count++ >= 10)
            gameObject.SetActive(false);
            
        for(int i = 0; i < mLists.Count; i++)
        {
            mLists[i].GetComponent<RectTransform>().DOSizeDelta(bigPos,speed,false).SetEase(Ease.OutCirc);
            yield return new WaitForSeconds(speed);
            StartCoroutine("setSizeDefault",mLists[i].GetComponent<RectTransform>());
        }

        StartCoroutine("loading");
    }

    IEnumerator setSizeDefault(RectTransform target)
    {
        float m_speed = speed*2;
        target.DOSizeDelta(smallPos,m_speed,false).SetEase(Ease.OutCirc);
        yield return new WaitForSeconds(m_speed);
        target.DOSizeDelta(endPos,m_speed,false).SetEase(Ease.OutCirc);
    }
    public void SpawnCircle()
    {
        for (int i = 0; i < numObjects; i++)
        {
            float a = i * (360.0f/numObjects);
            Vector3 pos = RandomCircle(center, radius ,a);
            GameObject temp = Instantiate(prefab, pos, Quaternion.identity,parent);
            temp.transform.rotation = Quaternion.Euler(new Vector3(0,0,360-a));
            temp.name = i.ToString();
        }
    }
    Vector3 RandomCircle(Vector3 center, float radius,float a)
    {
        Debug.Log(a);
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

    public void stopLoading()
    {
        StartCoroutine("setActiveFalseAfterSeconds");
    }

    public IEnumerator setActiveFalseAfterSeconds()
    {
        yield return new WaitForSeconds(Random.Range(0.2f,1f));
        this.gameObject.SetActive(false);
    }
}
