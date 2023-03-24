using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenBoost : MonoBehaviour
{
    Rigidbody2D rb;
    public bool isInject = false;
    public float speed = 100f;
    public float injectionAmount = 0.1f;
    public GameObject oxygenParticle;
    public float randomSeed = 0f;
    public GameObject childRenderer;
    public GameObject injectPoint;
    public Charactor ch;

    public float movePower = 10f;
    public float rotationPower = 10f;
    public int injectWay = 1;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A)){
            transform.position -= new Vector3(1.0f, 0.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= new Vector3(0.0f, 1.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(1.0f, 0.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0.0f, 1.0f, 0.0f);
        }










        // 스태미너가 있을때만 분사가능
        if(ch.Stamina > 0){
            if (isInject)
            {
                ch.Stamina -= 10f * Time.deltaTime;
            }
            else
            {
                if (ch.Stamina <= 100)
                {
                    ch.Stamina += 10f * Time.deltaTime;
                }
            }
            // float scroll = Input.GetAxis("Mouse ScrollWheel");
            // Debug.Log(scroll);

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

                if (IsMousePointerOnLeft(transform.up, mPosition))
                {
                    Debug.Log("마우스가 왼쪽에 있음");
                    injectWay = 1;
                    childRenderer.transform.localScale = new Vector3(-1, 1, 1);
                    injectPoint.transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    Debug.Log("마우스가 오른쪽에 있음");
                    injectWay = -1;
                    childRenderer.transform.localScale = new Vector3(1, 1, 1);
                    injectPoint.transform.localScale = new Vector3(1, 1, 1);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isInject = false;
                injectionAmount = 0f;
                oxygenParticle.SetActive(false);
                Debug.Log("분사 종료");
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                injectionAmount += 0.1f;
                var main = oxygenParticle.GetComponent<ParticleSystem>().main;
                main.startLifetime = 3 * injectionAmount;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                injectionAmount -= 0.1f;
                var main = oxygenParticle.GetComponent<ParticleSystem>().main;
                main.startLifetime = 3 * injectionAmount;
            }

            //injectionMovement();
            limtit_InjectionAmount();
            setInjectPoint();
        } else {    // 스태미나 없을땐 스태미나 회복
            ch.Stamina += 10f * Time.deltaTime;
        }
    }

    /// <summary>
    /// 산소 분사로 플레이어를 움직이는 함수
    /// </summary>
    private void injectionMovement()
    {
        //먼저 계산을 위해 마우스와 게임 오브젝트의 현재의 좌표를 임시로 저장합니다.
        Vector3 mPosition = Input.mousePosition; //마우스 좌표 저장

        // 목표 지점 흔들흔들
        mPosition += new Vector3(Random.Range(-randomSeed,randomSeed),Random.Range(-randomSeed,randomSeed),Random.Range(-randomSeed,randomSeed));

        Vector3 oPosition = transform.position; //게임 오브젝트 좌표 저장
        
        //카메라가 앞면에서 뒤로 보고 있기 때문에, 마우스 position의 z축 정보에 
        //게임 오브젝트와 카메라와의 z축의 차이를 입력시켜줘야 합니다.
        mPosition.z = oPosition.z - Camera.main.transform.position.z; 

        //화면의 픽셀별로 변화되는 마우스의 좌표를 유니티의 좌표로 변화해 줘야 합니다.
        //그래야, 위치를 찾아갈 수 있겠습니다.
        Vector3 target = Camera.main.ScreenToWorldPoint(mPosition);
        
        if(isInject)
        {
            transform.Translate(Vector3.left * injectionAmount * 20 * Time.deltaTime,Space.Self);
            //다음은 아크탄젠트(arctan, 역탄젠트)로 게임 오브젝트의 좌표와 마우스 포인트의 좌표를
            //이용하여 각도를 구한 후, 오일러(Euler)회전 함수를 사용하여 게임 오브젝트를 회전시키기
            //위해, 각 축의 거리차를 구한 후 오일러 회전함수에 적용시킵니다.
    
            //우선 각 축의 거리를 계산하여, dy, dx에 저장해 둡니다.
            float dy = target.y - oPosition.y;
            float dx = target.x - oPosition.x;
    
            //오릴러 회전 함수를 0에서 180 또는 0에서 -180의 각도를 입력 받는데 반하여
            //(물론 270과 같은 값의 입력도 전혀 문제없습니다.) 아크탄젠트 Atan2()함수의 결과 값은 
            //라디안 값(180도가 파이(3.141592654...)로)으로 출력되므로
            //라디안 값을 각도로 변화하기 위해 Rad2Deg를 곱해주어야 각도가 됩니다.
            float rotateDegree =  Mathf.Atan2(dy, dx)*Mathf.Rad2Deg;
    
            //구해진 각도를 오일러 회전 함수에 적용하여 z축을 기준으로 게임 오브젝트를 회전시킵니다.
            transform.rotation = Quaternion.Euler (0f, 0f, rotateDegree);
            childRenderer.transform.rotation = Quaternion.RotateTowards
                (childRenderer.transform.rotation, Quaternion.Euler (0f, 0f, rotateDegree), injectionAmount * speed * Time.deltaTime);
        }

        // if(-90 < transform.rotation.eulerAngles.z && transform.rotation.eulerAngles.z < 90)
        // {
        //     GetComponent<SpriteRenderer>().flipY = false;
        // }
        // else
        // {
        //     GetComponent<SpriteRenderer>().flipY = true;
        // }
        
        // if(IsMousePointerOnLeft(transform.up,mPosition))
        // {
        //     Debug.Log("마우스가 오른쪽에 있음");
        //     GetComponent<SpriteRenderer>().flipX = false;
            
        // }
        // else
        // {
        //     Debug.Log("마우스가 왼쪽에 있음");
        //     GetComponent<SpriteRenderer>().flipX = true;
        // }

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

    private void  setInjectPoint()
    {

        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 texturePosition = transform.InverseTransformPoint(worldPosition);

        float x = texturePosition.x / (GetComponent<SpriteRenderer>().bounds.size.x / 2);
        float y = texturePosition.y / (GetComponent<SpriteRenderer>().bounds.size.y / 2);

        x = Mathf.Clamp(x,-1f,1f);
        y = Mathf.Clamp(y,-1f,1f);

        Vector2 normalizedTexturePosition = new Vector2(x, y);
        injectPoint.transform.localPosition = new Vector3(0,y,0);
        // Debug.Log(normalizedTexturePosition);
        
        if(isInject)
        {
            GetComponent<Rigidbody2D>().AddTorque(rotationPower * -injectWay * injectPoint.transform.localPosition.y);
            GetComponent<Rigidbody2D>().AddForce(movePower * transform.right * injectWay * injectionAmount);
        }
        
    }
}
