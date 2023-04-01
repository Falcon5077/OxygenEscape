using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenSkill : MonoBehaviour
{
    public GameObject massBall;
    public GameObject explosion;
    public GameObject radiusObject;
    public GameObject explosionSmoke;
    public float rotateDegree;
    public float distance = 15f;
    public GameObject skillBtn;
    public Image coolTimeImage;
    public float maxCoolTime = 10f;
    public float nowCoolTime = 0f;
    public bool canSpawnBall = false;
    ClientObject co;
    // Start is called before the first frame update
    void Start()
    {
        co = GetComponent<ClientObject>();

        if(co.isMine){
            skillBtn = GameObject.Find("Skill1");
            coolTimeImage = GameObject.Find("CoolTime").GetComponent<Image>();
        }
    }

    public void calcCoolTime()
    {
        if(nowCoolTime > 0)
            nowCoolTime -= Time.deltaTime;
        else if(nowCoolTime <= 0)
            nowCoolTime = 0;

        coolTimeImage.fillAmount = nowCoolTime / maxCoolTime;
    }
    
    // Update is called once per frame
    void Update()
    {

        if(co.isMine == false)
            return;
            
        calcSpawnPos();
        calcCoolTime();
        
        if(Input.GetKeyDown(KeyCode.Q))
        {
            spawnBall();
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            spawnExplosion();
        }
    }

    public void spawnExplosion()
    {
        if(nowCoolTime > 0f)
            return;

        nowCoolTime = maxCoolTime;
        Vector3 way = new Vector2(radiusObject.transform.position.x,radiusObject.transform.position.y);
        way = transform.position - way;
        GetComponent<Rigidbody2D>().AddForce(way * 3,ForceMode2D.Impulse);
        GameObject smokeEffect = Instantiate(explosionSmoke,transform.position,Quaternion.identity);
        Destroy(smokeEffect, 1);
    }

    public void calcSpawnPos()
    {
        //먼저 계산을 위해 마우스와 게임 오브젝트의 현재의 좌표를 임시로 저장합니다.
        Vector3 mPosition = Input.mousePosition; //마우스 좌표 저장
        Vector3 oPosition = transform.position; //게임 오브젝트 좌표 저장
        
        //카메라가 앞면에서 뒤로 보고 있기 때문에, 마우스 position의 z축 정보에 
        //게임 오브젝트와 카메라와의 z축의 차이를 입력시켜줘야 합니다.
        mPosition.z = oPosition.z - Camera.main.transform.position.z; 

        //화면의 픽셀별로 변화되는 마우스의 좌표를 유니티의 좌표로 변화해 줘야 합니다.
        //그래야, 위치를 찾아갈 수 있겠습니다.
        Vector3 target = Camera.main.ScreenToWorldPoint(mPosition);

        //우선 각 축의 거리를 계산하여, dy, dx에 저장해 둡니다.
        float dy = target.y - oPosition.y;
        float dx = target.x - oPosition.x;

        //오릴러 회전 함수를 0에서 180 또는 0에서 -180의 각도를 입력 받는데 반하여
        //(물론 270과 같은 값의 입력도 전혀 문제없습니다.) 아크탄젠트 Atan2()함수의 결과 값은 
        //라디안 값(180도가 파이(3.141592654...)로)으로 출력되므로
        //라디안 값을 각도로 변화하기 위해 Rad2Deg를 곱해주어야 각도가 됩니다.
        float angle = Mathf.Atan2(dy,dx);

        rotateDegree =  Mathf.Atan2(dy, dx)*Mathf.Rad2Deg;

        Vector3 direction = new Vector3(Mathf.Cos(angle),Mathf.Sin(angle), 0);
        radiusObject.transform.position = transform.position + direction * distance;
        radiusObject.transform.rotation = Quaternion.Euler(0,0,rotateDegree);
    }
    public void spawnBall()
    {
        GameObject ball = Instantiate(massBall,radiusObject.transform.position,Quaternion.Euler (0f, 0f, rotateDegree));
        ball.GetComponent<massObject>().parent = this.gameObject;
    }

}
