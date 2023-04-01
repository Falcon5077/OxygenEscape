using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;

public class OxygenBoost : MonoBehaviour
{
    public bool isMine = false;
    public bool isRecv = false;

    public bool isInject = false;
    public float injectionAmount = 0.1f;
    public float randomSeed = 0f;

    public GameObject oxygenParticle;
    public GameObject childRenderer;
    public GameObject injectPoint;
    public float movePower = 10f;
    public float rotationPower = 10f;
    public int injectWay = 1;
    public float pre_y = 0f;
    public float rotateDegree;
    public Vector3 pos = Vector3.zero;
    public Vector3 rota = Vector3.zero;
    SpriteRenderer sr;
    public PlayerData pd;
    Rigidbody2D rb;

    public float lerpSpeed = 10f;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        pd = GetComponent<PlayerData>();
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        if(!isMine)
        {
            if(isRecv == true)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                
                transform.position = Vector3.Lerp(transform.position,pos,Time.deltaTime * lerpSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(rota),Time.deltaTime * lerpSpeed);
                injectPoint.transform.rotation = Quaternion.Lerp(injectPoint.transform.rotation,Quaternion.Euler (0f, 0f, rotateDegree),Time.deltaTime * lerpSpeed);
            }
            return;
        }

        if(Input.GetMouseButtonUp(0))
        {
            isInject = false;
            injectionAmount = 0f;
            oxygenParticle.SetActive(false);
        }

        // 스태미너가 있을때만 분사가능
        if(pd.Stamina > 0)
        {
            if (isInject)
            {
                pd.Stamina -= 10f * Time.deltaTime;

                if(pd.Stamina < 0)
                {
                    pd.Stamina = 0;
                    isInject = false;
                    injectionAmount = 0f;
                    oxygenParticle.SetActive(false);
                }
            }
            else
            {
                if (pd.Stamina <= 100)
                {
                    pd.Stamina += 7f * Time.deltaTime;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                isInject = true;
                injectionAmount = 0.5f;
                oxygenParticle.SetActive(true);
                var main = oxygenParticle.GetComponent<ParticleSystem>().main;
                main.startLifetime = 3 * injectionAmount;

                Debug.Log("분사 중");

                //먼저 계산을 위해 마우스와 게임 오브젝트의 현재의 좌표를 임시로 저장합니다.
                Vector3 mPosition = Input.mousePosition; //마우스 좌표 저장
                Vector3 oPosition = transform.position; //게임 오브젝트 좌표 저장

                //카메라가 앞면에서 뒤로 보고 있기 때문에, 마우스 position의 z축 정보에 
                //게임 오브젝트와 카메라와의 z축의 차이를 입력시켜줘야 합니다.
                mPosition.z = oPosition.z - Camera.main.transform.position.z;

                if(IsMousePointerOnLeft(transform.up,mPosition))
                {
                    injectWay = 1;
                    childRenderer.transform.localScale = new Vector3(-1,1,1);
                }
                else
                {
                    injectWay = -1;
                    childRenderer.transform.localScale = new Vector3(1,1,1);
                }
            }

            if(Input.GetKeyDown(KeyCode.A))
            {
                injectionAmount += 0.1f;
                var main = oxygenParticle.GetComponent<ParticleSystem>().main;
                main.startLifetime = 3 * injectionAmount;
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                injectionAmount -= 0.1f;
                var main = oxygenParticle.GetComponent<ParticleSystem>().main;
                main.startLifetime = 3 * injectionAmount;
            }

            limtit_InjectionAmount();
            setInjectPoint();
            clearPosition();
        } else {    // 스태미나 없을땐 스태미나 회복
            pd.Stamina += 7f * Time.deltaTime;
        }
    }

    public bool IsMousePointerOnLeft(Vector3 playerUpVector, Vector3 mousePosition)
    {
        // Calculate the direction of the mouse position relative to the player's up vector
        Vector3 direction = mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        direction = Vector3.ProjectOnPlane(direction, playerUpVector).normalized;

        // Check if the direction is on the left side of the player's up vector
        if (Vector3.Dot(Vector3.Cross(playerUpVector, direction), Vector3.forward) > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 산소 분사량을 0 에서 1 사이로 제한하는 함수
    /// </summary>
    private void limtit_InjectionAmount()
    {
        injectionAmount = Mathf.Clamp(injectionAmount,0f,1f);
    }

    /// <summary>
    /// 산소 분사로 플레이어를 움직이는 함수
    /// </summary>
    private void injectionMovement()
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
        rotateDegree =  Mathf.Atan2(dy, dx)*Mathf.Rad2Deg;
        rotateDegree = Mathf.Round(rotateDegree * 100) / 100;

        //구해진 각도를 오일러 회전 함수에 적용하여 z축을 기준으로 게임 오브젝트를 회전시킵니다.
        injectPoint.transform.rotation = Quaternion.Euler (0f, 0f, rotateDegree);
    }

    private void  setInjectPoint()
    {
        injectionMovement();
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 texturePosition = transform.InverseTransformPoint(worldPosition);

        float x = texturePosition.x / (sr.bounds.size.x / 2);
        float y = texturePosition.y / (sr.bounds.size.y / 2);

        x = Mathf.Clamp(x,-1f,1f);
        y = Mathf.Clamp(y,-1f,1f);

        Vector2 normalizedTexturePosition = new Vector2(x, y);
        injectPoint.transform.localPosition = new Vector3(0,y,0);
        
        // Debug.Log(normalizedTexturePosition);
        
        if(isInject)
        {
            GetComponent<Rigidbody2D>().AddTorque(rotationPower * -injectWay * injectPoint.transform.localPosition.y);
            GetComponent<Rigidbody2D>().AddForce(movePower * -injectPoint.transform.right * injectionAmount);
        }
    }

    public void recvValue(bool _isInject, float x, float y, float z,int _injectWay,float _injectionAmount,float _injectPointY,float _rotateDegree)
    {
        if(isMine)
            return;

        this.isRecv = true;

        this.injectWay = _injectWay;
        this.isInject = _isInject;
        this.injectionAmount = _injectionAmount;
        this.rotateDegree = _rotateDegree;

        float injectPointY = _injectPointY;

        oxygenParticle.SetActive(isInject);

        pos = new Vector3(x,y,0);
        rota = new Vector3(0,0,z);

        childRenderer.transform.localScale = new Vector3(-injectWay,1,1);
    }

    private void sendValue()
    {
        float injectPointY = Mathf.Round(injectPoint.transform.localPosition.y * 10) / 10;

        // float x = transform.position.x;
        // float y = transform.position.y;
        // float z = transform.eulerAngles.z;

        float x = Mathf.Round(transform.position.x * 100) / 100;
        float y = Mathf.Round(transform.position.y * 100) / 100;
        float z = Mathf.Round(transform.eulerAngles.z * 100) / 100;

        float _injectionAmount = Mathf.Round(injectionAmount * 100) / 100;
        float _rotateDegree = Mathf.Round(rotateDegree * 100) / 100;
        
        JSON jsonObject = new JSON();
        jsonObject.Add("type","sync_position");
        jsonObject.Add("sender",transform.name);
        jsonObject.Add("injectWay",injectWay);
        jsonObject.Add("isInject",isInject);
        jsonObject.Add("x",x);
        jsonObject.Add("y",y);
        jsonObject.Add("z",z);
        jsonObject.Add("injectionAmount",_injectionAmount);
        jsonObject.Add("injectPointY",injectPointY); 
        jsonObject.Add("rotateDegree",_rotateDegree);
        TCPManager.instance.SendMsg(jsonObject.CreateString());
    }

    IEnumerator sendPacket()
    {
        while(isMine)
        {
            sendValue();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void clearPosition()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().angularVelocity = 0;
            transform.position = new Vector3(Random.Range(-10f,10f),Random.Range(-4f,4f),0);
            transform.rotation = Quaternion.identity;

        }
    }
}
